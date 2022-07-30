using Microsoft.AspNetCore.Builder;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Strathweb.AspNetCore.AzureBlobFileProvider;

var builder = WebApplication.CreateBuilder(args);

#region Add CORS
builder.Services.AddCors(options => options.AddPolicy("Cors", builder =>
{
    builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
}));
#endregion

#region Add Authentication
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Tokens:Key"]));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters()
    {
        IssuerSigningKey = signingKey,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Tokens:Audience"],
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Tokens:Issuer"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});
#endregion

var blobOptionsTracks = new AzureBlobOptions
{
    ConnectionString = builder.Configuration.GetConnectionString("BlobConnectionString"),
    DocumentContainer = "tracks"
};
var azureBlobFileProviderTracks = new AzureBlobFileProvider(blobOptionsTracks);

var blobOptionsImages = new AzureBlobOptions
{
    ConnectionString = builder.Configuration.GetConnectionString("BlobConnectionString"),
    DocumentContainer = "images"
};
var azureBlobFileProviderImages = new AzureBlobFileProvider(blobOptionsImages);

var composite = new CompositeFileProvider(azureBlobFileProviderTracks, azureBlobFileProviderImages);
builder.Services.AddSingleton(composite);

builder.Services.AddMvc().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();

var compositeFileProvider = app.Services.GetRequiredService<CompositeFileProvider>();

app.UseCors("Cors");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dogs Resource Server API V1");
    c.RoutePrefix = string.Empty;
});
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();