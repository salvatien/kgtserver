﻿using Dogs.Identity.Api;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();