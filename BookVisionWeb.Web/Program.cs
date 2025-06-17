using BookVisionWeb.Interface;
using BookVisionWeb.UseCase;
using BookVisionWeb.Infrastructure;

DotNetEnv.Env.Load(); // .env を先にロード！

var builder = WebApplication.CreateBuilder(args);

// ここで .env から値を拾う
var connString = Environment.GetEnvironmentVariable("BOOKVISIONWEB_DB");
if (string.IsNullOrWhiteSpace(connString))
    throw new Exception("BOOKVISIONWEB_DBが未設定です。");

// UseCase
builder.Services.AddScoped<IRecognizePageUseCase, RecognizePageInteractor>();
// Interface
builder.Services.AddScoped<RecognizePageJsonPresenter>();
// Infrastructure: PostgreSQLへ切替
builder.Services.AddScoped<IPageRepository>(provider =>
    new PostgresPageRepository(connString!));
builder.Services.AddSingleton<IOcrGateway, TesseractGateway>();

var app = builder.Build();
app.MapOcr();
app.MapUploadForm();
app.Run("http://localhost:5040");
