using Core;
using Core.Modeles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Core.Security;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContextPool<AppDbContext>
(options => options.UseSqlServer(builder.Configuration.GetConnectionString("PapyDBConnection")));

builder.Services.AddMvc((opt) => { 
    opt.EnableEndpointRouting = false;
    var policy=new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
} ).AddXmlSerializerFormatters();


builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = "129907699626-v42mje98mefdd4brp530vkvhljod8q52.apps.googleusercontent.com";
    options.ClientSecret = "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";
}).AddFacebook(options =>
{
    options.AppId = "215825291209948";
    options.AppSecret = "e15a1751a072fbecf2d77cc047d67532";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EditUserRolesClaims", policy => policy.AddRequirements(new ManageAdminRolesClaimsRequirements()));
});

builder.Services.AddAuthorization(options =>
options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role", "True")));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EditRolePolicy", policy => policy.RequireClaim("Edit Role", "True"));
});

builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy("AllAllowedPolicy", policy => policy.RequireRole("Super Admin"));
    options.AddPolicy("AllAllowedPolicy", policy => policy.RequireUserName("baby@gmail.com"));
});

builder.Services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminsRolesClaims>();
builder.Services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();

builder.Services.AddScoped<ITeamMembersRepository, SQLTeamMembersRepository>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(opt => { 
    opt.Password.RequiredLength = 8; 
    opt.Password.RequireLowercase = true;
    opt.Password.RequireUppercase = true;


    opt.SignIn.RequireConfirmedEmail = true;

    opt.Lockout.MaxFailedAccessAttempts = 5;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
}
).AddDefaultTokenProviders()
 .AddEntityFrameworkStores<AppDbContext>();


builder.Services.AddLogging((logging) =>
{
   var logConfig= builder.Configuration.GetSection("Logging");
    logging.AddConfiguration(logConfig);
    logging.AddConsole();
    logging.AddDebug();
    logging.AddEventSourceLogger();
    logging.AddNLog();
});

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
   app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
}

app.UseStaticFiles();

app.UseAuthentication();
app.UseMvc((rout) =>
{
    rout.MapRoute("default", "{Controller=Home}/{Action=Welcome}/{id?}");
});


app.Run();
