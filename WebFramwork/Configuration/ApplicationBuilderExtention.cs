using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebFramework.Configuration
{
    public static class ApplicationBuilderExtention
    {
        public static void UseHsts(this IApplicationBuilder app, IHostEnvironment env)
        {
            if (!env.IsDevelopment())
                app.UseHsts();
        }
    }
}
