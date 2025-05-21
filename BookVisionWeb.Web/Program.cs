using BookVisionWeb.Interface;
using BookVisionWeb.UseCase;
using BookVisionWeb.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// DI: UseCase
builder.Services.AddScoped<IRecognizePageUseCase, RecognizePageInteractor>();
// Interface
builder.Services.AddScoped<RecognizePageJsonPresenter>();
// Infrastructure
builder.Services.AddSingleton<IPageRepository, InMemoryPageRepository>();
builder.Services.AddSingleton<IOcrGateway, TesseractGateway>();

var app = builder.Build();
app.MapOcr();                       // Interface で定義した拡張メソッド
app.MapUploadForm();
app.Run("http://localhost:5040");
