using KoiFarmShop.Data;
using KoiFarmShop.GRPC.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<KoiFishGRPCServices>();

app.MapGet("/", () => "KoiFish Proto Service initialize success!");

app.Run();
