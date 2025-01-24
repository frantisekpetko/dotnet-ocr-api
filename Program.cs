using Tesseract;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;

// Add services to the container.

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        /*
        app.MapGet("api/ocr", () =>
        {
            byte[] imageArray = File.ReadAllBytes(@"2.png");
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            File.WriteAllBytes(@"_choices.png", Convert.FromBase64String(base64ImageRepresentation));


            var engine = new TesseractEngine(@"C:\tessdata", "ces");
            var image = Pix.LoadFromFile(@"_choices.png");

            var page = engine.Process(image);

            var text = page.GetText();
            Console.Write(text);
            string[] lines = text.Split(
            new string[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None).Where(x => !string.IsNullOrEmpty(x)).ToArray();

            List<string[]> _data = new List<string[]>
            {
                lines
            };
            var json = JsonConvert.SerializeObject(_data, Formatting.Indented);
            File.WriteAllText(@"text.json", json, Encoding.UTF8);
            return json;

        });
        */

        app.MapPost("api/ocr", (Image image) =>
        {
            var engine = new TesseractEngine(@"C:\tessdata", "ces");
            var imageData = Pix.LoadFromMemory(Convert.FromBase64String(image.ImageString));

            var page = engine.Process(imageData);

            var text = page.GetText();
            Console.Write(text);
            string[] lines = text.Split(
            new string[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None).Where(x => !string.IsNullOrEmpty(x)).ToArray();

            List<string[]> data = new List<string[]>
            {
                lines
            };

            return Results.Ok(data);

        });

        app.Run();
    }
}

public class Image {
    public string? ImageString { get; set; }
}