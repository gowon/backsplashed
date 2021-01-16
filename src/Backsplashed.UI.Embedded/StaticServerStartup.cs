namespace Backsplashed.UI.Embedded
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Hosting;

    public class StaticServerStartup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var fileProvider = new ManifestEmbeddedFileProvider(GetType().Assembly, "Assets");

            app.UseDefaultFiles(new DefaultFilesOptions
            {
                FileProvider = fileProvider
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = fileProvider, ServeUnknownFileTypes = true
            });
        }
    }
}