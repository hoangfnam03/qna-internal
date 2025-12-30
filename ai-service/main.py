import os
from typing import List
from fastapi import FastAPI
from pydantic import BaseModel, Field
from fastapi.middleware.cors import CORSMiddleware
import torch
import numpy as np
from transformers import AutoTokenizer, AutoModel, AutoModelForCausalLM

try:
    from peft import PeftModel
    HAS_PEFT = True
except Exception:
    HAS_PEFT = False

APP_NAME = "qna-ai-service"

# Embed model nhỏ (miễn phí, tốt cho search): e5-small-v2
EMBED_MODEL = os.getenv("EMBED_MODEL", "intfloat/e5-small-v2")

# LLM nhỏ để generate demo
LLM_BASE_MODEL = os.getenv("LLM_BASE_MODEL", "Qwen/Qwen2.5-0.5B-Instruct")
ADAPTER_PATH = os.getenv("LLM_ADAPTER_PATH", "").strip()

DEVICE = os.getenv("DEVICE", "auto")  # auto|cpu|cuda
DTYPE = os.getenv("DTYPE", "auto")    # auto|float16|bfloat16|float32
MAX_EMBED_BATCH = int(os.getenv("MAX_EMBED_BATCH", "16"))


def pick_device() -> str:
    if DEVICE == "cpu":
        return "cpu"
    if DEVICE == "cuda":
        return "cuda" if torch.cuda.is_available() else "cpu"
    return "cuda" if torch.cuda.is_available() else "cpu"


def pick_dtype():
    if DTYPE == "float16":
        return torch.float16
    if DTYPE == "bfloat16":
        return torch.bfloat16
    if DTYPE == "float32":
        return torch.float32
    # auto
    if torch.cuda.is_available():
        return torch.float16
    return torch.float32


def mean_pool(last_hidden_state: torch.Tensor, attention_mask: torch.Tensor) -> torch.Tensor:
    mask = attention_mask.unsqueeze(-1).type_as(last_hidden_state)
    summed = (last_hidden_state * mask).sum(dim=1)
    counts = mask.sum(dim=1).clamp(min=1e-9)
    return summed / counts


app = FastAPI(title=APP_NAME)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # dev thôi
    allow_credentials=False,
    allow_methods=["*"],
    allow_headers=["*"],
)

_device = pick_device()
_dtype = pick_dtype()

# ----- Load embedding model -----
_embed_tokenizer = AutoTokenizer.from_pretrained(EMBED_MODEL, use_fast=True)
_embed_model = AutoModel.from_pretrained(EMBED_MODEL, torch_dtype=_dtype)
_embed_model.eval()
_embed_model = _embed_model.to(_device)

EMBED_DIM = int(getattr(_embed_model.config, "hidden_size", 0) or 0)

# ----- Load LLM -----
_llm_tokenizer = AutoTokenizer.from_pretrained(LLM_BASE_MODEL, use_fast=True)
_llm = AutoModelForCausalLM.from_pretrained(
    LLM_BASE_MODEL,
    torch_dtype=_dtype,
    device_map="auto" if _device == "cuda" else None
)
if _device == "cpu":
    _llm = _llm.to("cpu")

if ADAPTER_PATH:
    if not HAS_PEFT:
        raise RuntimeError("LLM_ADAPTER_PATH is set but peft is not installed.")
    _llm = PeftModel.from_pretrained(_llm, ADAPTER_PATH)

_llm.eval()


class EmbedRequest(BaseModel):
    texts: List[str] = Field(..., min_items=1)


class EmbedResponse(BaseModel):
    model: str
    dim: int
    vectors: List[List[float]]


class GenerateRequest(BaseModel):
    prompt: str
    max_new_tokens: int = 256
    temperature: float = 0.2
    top_p: float = 0.9
    do_sample: bool = True


class GenerateResponse(BaseModel):
    model: str
    text: str


@app.get("/health")
def health():
    return {"ok": True, "service": APP_NAME}


@app.get("/info")
def info():
    return {
        "embed_model": EMBED_MODEL,
        "embed_dim": EMBED_DIM,
        "llm_base_model": LLM_BASE_MODEL,
        "adapter": ADAPTER_PATH if ADAPTER_PATH else None,
        "device": _device,
        "dtype": str(_dtype)
    }


@app.post("/embed", response_model=EmbedResponse)
def embed(req: EmbedRequest):
    texts = [(t or "").strip() for t in req.texts]
    # e5 khuyến nghị prefix "query:" / "passage:"
    # bạn có thể prefix theo ngữ cảnh, ở đây đơn giản:
    inputs = _embed_tokenizer(
        texts,
        padding=True,
        truncation=True,
        max_length=512,
        return_tensors="pt"
    )
    inputs = {k: v.to(_device) for k, v in inputs.items()}

    with torch.no_grad():
        out = _embed_model(**inputs)
        pooled = mean_pool(out.last_hidden_state, inputs["attention_mask"])

        # normalize cosine
        pooled = torch.nn.functional.normalize(pooled, p=2, dim=1)

    vectors = pooled.detach().cpu().numpy().astype(np.float32).tolist()
    return EmbedResponse(model=EMBED_MODEL, dim=EMBED_DIM, vectors=vectors)


@app.post("/generate", response_model=GenerateResponse)
def generate(req: GenerateRequest):
    inputs = _llm_tokenizer(req.prompt, return_tensors="pt", truncation=True, max_length=2048)
    if _device == "cuda":
        inputs = {k: v.to(_llm.device) for k, v in inputs.items()}
    else:
        inputs = {k: v.to("cpu") for k, v in inputs.items()}

    with torch.no_grad():
        out = _llm.generate(
            **inputs,
            max_new_tokens=req.max_new_tokens,
            temperature=req.temperature,
            top_p=req.top_p,
            do_sample=req.do_sample
        )

    text = _llm_tokenizer.decode(out[0], skip_special_tokens=True)
    return GenerateResponse(model=LLM_BASE_MODEL, text=text)
