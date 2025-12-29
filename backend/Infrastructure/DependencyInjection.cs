using Application.AI.Options;
using Application.Common.Interfaces;
using Infrastructure.AIClients;
using Infrastructure.Email;
using Infrastructure.FileStorage;
using Infrastructure.Identity.Jwt;
using Infrastructure.Identity.PasswordHashing;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.VectorStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ---- Interceptors ----
            services.AddScoped<AuditableEntitySaveChangesInterceptor>();
            services.AddScoped<SoftDeleteSaveChangesInterceptor>();
            
            // ---- DbContext ----
            services.AddDbContext<ApplicationDbContext>((sp, opt) =>
            {
                var cs = configuration.GetConnectionString("DefaultConnection");
                opt.UseSqlServer(cs, sql =>
                {
                    sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                });

                opt.AddInterceptors(
                    sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>(),
                    sp.GetRequiredService<SoftDeleteSaveChangesInterceptor>()
                );
            });

            // IApplicationDbContext -> ApplicationDbContext
            services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

            // ---- Auth/JWT (service riêng, không dùng Identity EF) ----
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

            // ---- Password hashing ----
            services.AddSingleton<IPasswordHasherService, PasswordHasherService>();

            // ---- Email ----
            services.Configure<SmtpOptions>(configuration.GetSection("Email:Smtp"));
            services.AddSingleton<IEmailSender, SmtpEmailSender>();

            // ---- File storage ----
            services.Configure<LocalFileStorageOptions>(configuration.GetSection("FileStorage:Local"));
            services.AddSingleton<IFileStorage, LocalFileStorage>();

            // ---- AI / Vector store (nếu bạn đã có interface) ----
            // services.AddHttpClient<IEmbeddingClient, EmbeddingHttpClient>(...);
            // services.AddHttpClient<IInferenceClient, InferenceHttpClient>(...);
            
            services.Configure<AiServiceOptions>(configuration.GetSection("AIService"));
            services.Configure<QdrantOptions>(configuration.GetSection("Qdrant"));
            
            services.AddHttpClient<IAiServiceClient,AiServiceClient>();
            services.AddHttpClient<IVectorStore,QdrantVectorStore>();
            services.AddSingleton<IVectorStore, QdrantVectorStore>();

            return services;
        }
    }
}
