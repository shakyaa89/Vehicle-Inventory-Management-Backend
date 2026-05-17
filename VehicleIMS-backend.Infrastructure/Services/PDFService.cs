using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;

namespace VehicleIMS_backend.Infrastructure.Services
{
    // Service to generate PDF reports
    public class PDFService : IPDFService
    {
        // Generate a financial report PDF from data model
        public byte[] GenerateFinancialReport(FinancialReportDTO model)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);

                    page.DefaultTextStyle(x => x.FontSize(10));

                    // HEADER
                    page.Header().Column(col =>
                    {
                        col.Item()
                            .Text(model.Title)
                            .FontSize(22)
                            .Bold();

                        col.Item()
                            .Text($"Period: {model.From:yyyy-MM-dd} → {model.To:yyyy-MM-dd}")
                            .FontSize(11)
                            .FontColor(Colors.Grey.Darken1);

                        col.Item()
                            .PaddingTop(10)
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten2);
                    });

                    // CONTENT
                    page.Content().Column(col =>
                    {
                        col.Spacing(15);

                        // SUMMARY
                        col.Item()
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(10)
                            .Column(summary =>
                            {
                                summary.Item().Text("Summary").Bold().FontSize(14);

                                summary.Item().Column(c =>
                                {
                                    c.Item().Text($"Total Sales: {model.TotalSales:N2}");
                                    c.Item().Text($"Total Purchases: {model.TotalPurchases:N2}");
                                    c.Item().Text($"Net Profit: {model.NetProfit:N2}").Bold();
                                });
                            });

                        // SALES
                        col.Item().Text("Sales").FontSize(14).Bold();

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn(); 
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyleHeader).Text("ID");
                                header.Cell().Element(CellStyleHeader).Text("Reference");
                                header.Cell().Element(CellStyleHeader).Text("Amount");
                                header.Cell().Element(CellStyleHeader).Text("Date");
                                header.Cell().Element(CellStyleHeader).Text("Customer");
                            });

                            foreach (var item in model.Sales)
                            {
                                table.Cell().Element(CellStyle).Text(item.Id);
                                table.Cell().Element(CellStyle).Text(item.Reference);
                                table.Cell().Element(CellStyle).Text($"{item.Amount:N2}");
                                table.Cell().Element(CellStyle).Text(item.Date.ToString("yyyy-MM-dd"));
                                table.Cell().Element(CellStyle).Text(item.CustomerName ?? "-");
                            }
                        });

                        col.Item().PaddingTop(10);

                        // PURCHASES
                        col.Item().Text("Purchases").FontSize(14).Bold();

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyleHeader).Text("ID");
                                header.Cell().Element(CellStyleHeader).Text("Reference");
                                header.Cell().Element(CellStyleHeader).Text("Amount");
                                header.Cell().Element(CellStyleHeader).Text("Date");
                                header.Cell().Element(CellStyleHeader).Text("Vendor");
                            });

                            foreach (var item in model.Purchases)
                            {
                                table.Cell().Element(CellStyle).Text(item.Id);
                                table.Cell().Element(CellStyle).Text(item.Reference);
                                table.Cell().Element(CellStyle).Text($"{item.Amount:N2}");
                                table.Cell().Element(CellStyle).Text(item.Date.ToString("yyyy-MM-dd"));
                                table.Cell().Element(CellStyle).Text(item.VendorName ?? "-");
                            }
                        });
                    });

                    // FOOTER
                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Generated on ");
                            text.Span(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                        });
                });
            });

            return document.GeneratePdf();
        }

        private static IContainer CellStyle(IContainer container)
        {
            return container
                .PaddingVertical(5)
                .PaddingHorizontal(3);
        }

        private static IContainer CellStyleHeader(IContainer container)
        {
            return container
                .Background(Colors.Grey.Lighten3)
                .PaddingVertical(5)
                .PaddingHorizontal(3)
                .DefaultTextStyle(x => x.SemiBold());
        }
    }
}