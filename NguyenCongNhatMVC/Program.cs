using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;

namespace NguyenCongNhatMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "FUNewsManagementSystem-Keys")));
            builder.Services.AddDbContext<Data.FUNewsDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.Configure<Services.AdminAccountOptions>(builder.Configuration.GetSection("AdminAccount"));
            builder.Services.Configure<Services.EmailOptions>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped(typeof(Repositories.IRepository<>), typeof(Repositories.Repository<>));
            builder.Services.AddScoped<Repositories.IUnitOfWork, Repositories.UnitOfWork>();
            builder.Services.AddScoped<Services.IAuthService, Services.AuthService>();
            builder.Services.AddScoped<Services.IEmailSender, Services.SmtpEmailSender>();
            builder.Services.AddScoped<Services.IEmailVerificationService, Services.EmailVerificationService>();
            builder.Services.AddScoped<Services.ICurrentUserService, Services.CurrentUserService>();
            builder.Services.AddScoped<Services.IAuditService, Services.AuditService>();
            builder.Services.AddScoped<Services.IAccountService, Services.AccountService>();
            builder.Services.AddScoped<Services.ICategoryService, Services.CategoryService>();
            builder.Services.AddScoped<Services.INewsService, Services.NewsService>();
            builder.Services.AddScoped<Services.ITagService, Services.TagService>();
            builder.Services.AddScoped<Services.IAnalyticsService, Services.AnalyticsService>();
            builder.Services.AddScoped<Services.IArticleWorkflowService, Services.ArticleWorkflowService>();
            builder.Services.AddScoped<Services.INotificationService, Services.NotificationService>();
            builder.Services.AddScoped<Services.IArticleInteractionService, Services.ArticleInteractionService>();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}/{id?}");

            Data.DbInitializer.Initialize(app.Services);
            app.Run();
        }
    }
}
