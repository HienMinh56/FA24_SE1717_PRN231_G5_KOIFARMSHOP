using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using KoiFarmShop.Service.CoreWCFServices; // Ensure proper using

var builder = WebApplication.CreateBuilder(args);

// Add WCF services to the DI container
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseServiceModel(builder =>
{
    builder.AddService<KoiFishWCFService>();
    builder.AddService<VoucherWCFService>();

    builder.AddServiceEndpoint<KoiFishWCFService, IKoiFishWCFService>(new BasicHttpBinding(), "/KoiFishWCFService.svc");
    builder.AddServiceEndpoint<VoucherWCFService, IVoucherWCFService>(new BasicHttpBinding(), "/VoucherWCFService.svc");

    // Enable metadata publishing (for WSDL generation)
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
    serviceMetadataBehavior.HttpsGetEnabled = true;  // Adjust as needed
});

app.Run();
