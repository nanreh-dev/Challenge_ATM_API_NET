using ATM.Helpers;
using ATM.Interfaces;
using ATM.Models;
using ATM.Models.CustomModels;
using ATM.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "ATM_API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Ingrese token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
var option = new RewriteOptions();
option.AddRedirect("^$", "swagger");

//inyeccion del context
builder.Services.AddDbContext<ATMContext>(opt =>
{
    var sqlConnection = builder.Configuration.GetConnectionString("ATM");
    opt.UseSqlServer(sqlConnection);
});

//inyeccion de servicios
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITransactionService, TransactionServices>();


//jwt
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRewriter(option);
app.UseAuthentication();
app.UseAuthorization();

//endpoints
app.MapPost("/login", async (HttpContext ctx, LoginModel model, ILoginService _loginService) =>
{
    try
    {
        var cardState = _loginService.CheckCard(model);

        if (cardState.Success)
        {
            var token = GetToken.Get(builder.Configuration["Jwt:Key"], builder.Configuration["Jwt:Issuer"], model);

            await ctx.Response.WriteAsJsonAsync(new { Token = token });
        }
        else
        {
            ctx.Response.StatusCode = StatusCodes.Status404NotFound;
            await ctx.Response.WriteAsJsonAsync(new { Error = cardState.Error });
        }
    }
    catch (Exception ex)
    {
        throw new Exception("Error: " + ex.Message);
    }
});

app.MapPost("/saldo", async (HttpContext ctx, ITransactionService _transactionService, IUserService _userService) =>
{
    try
    {
        var identity = ctx.User.Identity as ClaimsIdentity;

        if (identity != null)
        {
            //obtengo el numero de tarjeta en el token
            string cardNumber = identity.Claims.First().Value;

            var user = _userService.Get(cardNumber);

            //compruebo que no sea null y aunq tenga token no este bloqueado
            if (user == null)
            {
                ctx.Response.StatusCode = StatusCodes.Status404NotFound;
                await ctx.Response.WriteAsJsonAsync(new { Error = Constants.Errors.NO_CARD });
                return;
            }
            else if (!user.Enabled)
            {
                ctx.Response.StatusCode = StatusCodes.Status203NonAuthoritative;
                await ctx.Response.WriteAsJsonAsync(new { Error = Constants.Errors.CARD_BLOQUED });
                return;
            }
            else
            {
                var transaction = _transactionService.GetSaldo(cardNumber);

                await ctx.Response.WriteAsJsonAsync(new { Transaction = transaction });
                return;
            }
        }
        ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await ctx.Response.WriteAsJsonAsync(new { Error = Constants.Errors.TRANSACTION_ERROR });
    }
    catch (Exception ex)
    {
        throw new Exception("Error: " + ex.Message);
    }
}).RequireAuthorization();

app.MapPost("/deposito/{cantidad}", async (decimal cantidad, HttpContext ctx, ITransactionService _transactionService, IUserService _userService) =>
{
    try
    {
        var identity = ctx.User.Identity as ClaimsIdentity;

        if (identity != null)
        {
            string cardNumber = identity.Claims.First().Value;

            var user = _userService.Get(cardNumber);

            if (user == null)
            {
                ctx.Response.StatusCode = StatusCodes.Status404NotFound;
                await ctx.Response.WriteAsJsonAsync(new { Error = Constants.Errors.NO_CARD });
                return;
            }
            else if (!user.Enabled)
            {
                ctx.Response.StatusCode = StatusCodes.Status203NonAuthoritative;
                await ctx.Response.WriteAsJsonAsync(new { Error = Constants.Errors.CARD_BLOQUED });
                return;
            }
            else
            {
                var transaction = _transactionService.Transactions(cardNumber, Constants.TransactionType.CASH_IN, cantidad);

                await ctx.Response.WriteAsJsonAsync(new { Transaction = transaction });
                return;
            }
        }
        ctx.Response.StatusCode = StatusCodes.Status404NotFound;
        await ctx.Response.WriteAsJsonAsync(new { Error = Constants.Errors.TRANSACTION_ERROR });
    }
    catch (Exception ex)
    {
        throw new Exception("Error: " + ex.Message);
    }
}).RequireAuthorization();

app.MapPost("/extraccion/{cantidad}", async (decimal cantidad, HttpContext ctx, ITransactionService _transactionService, IUserService _userService) =>
{
    try
    {
        var identity = ctx.User.Identity as ClaimsIdentity;

        if (identity != null)
        {
            string cardNumber = identity.Claims.First().Value;

            var user = _userService.Get(cardNumber);

            if (user == null)
            {
                ctx.Response.StatusCode = StatusCodes.Status404NotFound;
                await ctx.Response.WriteAsJsonAsync(new { Error = Constants.Errors.NO_CARD });
                return;
            }
            else if (!user.Enabled)
            {
                ctx.Response.StatusCode = StatusCodes.Status203NonAuthoritative;
                await ctx.Response.WriteAsJsonAsync(new { Error = Constants.Errors.CARD_BLOQUED });
                return;
            }
            else
            {
                var transaction = _transactionService.Transactions(cardNumber, Constants.TransactionType.CASH_OUT, cantidad);

                await ctx.Response.WriteAsJsonAsync(new { Transaction = transaction });
                return;
            }
        }
        ctx.Response.StatusCode = StatusCodes.Status404NotFound;
        await ctx.Response.WriteAsJsonAsync(new { Error = Constants.Errors.TRANSACTION_ERROR });
    }
    catch (Exception ex)
    {
        throw new Exception("Error: " + ex.Message);
    }

}).RequireAuthorization();

app.MapGet("/movimientos/{pagina}", async (string pagina, HttpContext ctx, IUserService _userService, ITransactionService _transactionService) =>
{
    var identity = ctx.User.Identity as ClaimsIdentity;

    if (identity != null)
    {
        string cardNumber = identity.Claims.First().Value;

        var user = _userService.Get(cardNumber);

        if (user == null)
        {
            ctx.Response.StatusCode = StatusCodes.Status404NotFound;
            await ctx.Response.WriteAsJsonAsync(new { Error = Constants.Errors.NO_CARD });
            return;
        }
        else if (!user.Enabled)
        {
            ctx.Response.StatusCode = StatusCodes.Status203NonAuthoritative;
            await ctx.Response.WriteAsJsonAsync(new { Error = Constants.Errors.CARD_BLOQUED });
            return;
        }
        else
        {
            var movements = _transactionService.GetMovements(pagina, user);
            await ctx.Response.WriteAsJsonAsync(new { Movements = movements });
            return;
        }
    }
    ctx.Response.StatusCode = StatusCodes.Status404NotFound;
    await ctx.Response.WriteAsJsonAsync(new { Error = Constants.Errors.TRANSACTION_ERROR });
}).RequireAuthorization();

app.Run();