using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GameServer.Hubs;
using GameServer.Services.Core.SignalR;
using GameServer.Services.Gameplay.Rooms;
using GameServer.Services.Gameplay.Matches;
using GameServer.Services.Gameplay.Players;

namespace GameServer
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IMatchService, MatchService>();
            services.AddSingleton<IPlayerService, PlayerService>();
            services.AddSingleton<IPlayerHeartbeatTracker, PlayerHeartbeatTracker>();
            services.AddSingleton<IRoomService, RoomService>();
            services.AddSingleton<IUserIdProvider, UserIdProvider>();

            services.AddSignalR(options =>
                    {
                        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                        options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                    })
                    .AddMessagePackProtocol();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = Configuration["JWT_ISSUER"],
                            ValidAudience = Configuration["JWT_AUDIENCE"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT_SECRET_KEY"]!))
                        };
                    })
                    .AddCookie("Cookie");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MatchHub>("hubs/match");
                endpoints.MapControllers();
            });
        }
    }
}
