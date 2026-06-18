using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HR.LeaveManagement.Application
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            // Lấy assembly hiện tại
            var executingAssembly = typeof(ApplicationServicesRegistration).Assembly;

            // THAY ĐỔI Ở ĐÂY: Thêm "cfg => { }," vào trước biến executingAssembly
            services.AddAutoMapper(cfg => { }, executingAssembly);

            // Cấu hình MediatR (Giữ nguyên vì đã chuẩn bản mới)
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(executingAssembly));

            return services;
        }
    }
}