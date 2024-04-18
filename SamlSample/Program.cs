using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SamlSample;
using SamlSample.Extensions;
using Sustainsys.Saml2;
using Sustainsys.Saml2.Metadata;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddAuthentication(AuthenticationScheme.Saml)
    .AddCookie(AuthenticationScheme.Saml, options => {
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.Path = "/";
    })
    .AddSaml2(options  =>
    {
        var samlConfig = builder.Configuration.GetSection("Authentication:Saml").Get<SamlConfiguration>();

        options.SPOptions.PublicOrigin = samlConfig.EntityId.GetRoot();
        options.SPOptions.EntityId = new EntityId(samlConfig.EntityId);
        options.SPOptions.ReturnUrl = new Uri("/", UriKind.Relative);

        var idp = new IdentityProvider(new EntityId(samlConfig.IdentityProviderIssuer), options.SPOptions)
        {
            MetadataLocation = samlConfig.MetadataUrl,
            AllowUnsolicitedAuthnResponse = true,
        };

        options.IdentityProviders.Add(idp);
    });
    
builder.Services.AddRazorPages();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app
    .UseAuthentication()
    .UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});

app.Run();