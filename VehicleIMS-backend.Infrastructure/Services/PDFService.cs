using System;
using System.Collections.Generic;
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

        public byte[] GenerateRegularCustomersReport(List<RegularCustomerReportDTO> customers, DateTime from, DateTime to)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("Regular Customers Report").FontSize(22).Bold();
                        col.Item().Text($"Period: {from:yyyy-MM-dd} → {to:yyyy-MM-dd}")
                            .FontSize(11)
                            .FontColor(Colors.Grey.Darken1);
                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    page.Content().Column(col =>
                    {
                        col.Spacing(12);
                        col.Item().Text($"Total customers: {customers.Count}").FontSize(11);

                        if (customers.Count == 0)
                        {
                            col.Item().Text("No data available for the selected period.");
                            return;
                        }

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1.4f);
                                columns.RelativeColumn(0.9f);
                                columns.RelativeColumn(1.1f);
                                columns.RelativeColumn(1.1f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyleHeader).Text("ID");
                                header.Cell().Element(CellStyleHeader).Text("Name");
                                header.Cell().Element(CellStyleHeader).Text("Email");
                                header.Cell().Element(CellStyleHeader).Text("Phone");
                                header.Cell().Element(CellStyleHeader).Text("Visits");
                                header.Cell().Element(CellStyleHeader).Text("Total spent");
                                header.Cell().Element(CellStyleHeader).Text("Last purchase");
                            });

                            foreach (var customer in customers)
                            {
                                table.Cell().Element(CellStyle).Text(customer.Id);
                                table.Cell().Element(CellStyle).Text(BuildDisplayName(customer.FullName, customer.UserName));
                                table.Cell().Element(CellStyle).Text(customer.Email ?? "-");
                                table.Cell().Element(CellStyle).Text(customer.PhoneNumber ?? "-");
                                table.Cell().Element(CellStyle).Text(customer.VisitCount);
                                table.Cell().Element(CellStyle).Text($"{customer.TotalSpent:N2}");
                                table.Cell().Element(CellStyle).Text(FormatDate(customer.LastPurchaseAt));
                            }
                        });
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Generated on ");
                            text.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                        });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GenerateHighSpendersReport(List<HighSpenderReportDTO> customers, DateTime from, DateTime to)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("High Spenders Report").FontSize(22).Bold();
                        col.Item().Text($"Period: {from:yyyy-MM-dd} → {to:yyyy-MM-dd}")
                            .FontSize(11)
                            .FontColor(Colors.Grey.Darken1);
                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    page.Content().Column(col =>
                    {
                        col.Spacing(12);
                        col.Item().Text($"Total customers: {customers.Count}").FontSize(11);

                        if (customers.Count == 0)
                        {
                            col.Item().Text("No data available for the selected period.");
                            return;
                        }

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1.4f);
                                columns.RelativeColumn(1.1f);
                                columns.RelativeColumn(0.9f);
                                columns.RelativeColumn(1.1f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyleHeader).Text("ID");
                                header.Cell().Element(CellStyleHeader).Text("Name");
                                header.Cell().Element(CellStyleHeader).Text("Email");
                                header.Cell().Element(CellStyleHeader).Text("Phone");
                                header.Cell().Element(CellStyleHeader).Text("Total spent");
                                header.Cell().Element(CellStyleHeader).Text("Visits");
                                header.Cell().Element(CellStyleHeader).Text("Last purchase");
                            });

                            foreach (var customer in customers)
                            {
                                table.Cell().Element(CellStyle).Text(customer.Id);
                                table.Cell().Element(CellStyle).Text(BuildDisplayName(customer.FullName, customer.UserName));
                                table.Cell().Element(CellStyle).Text(customer.Email ?? "-");
                                table.Cell().Element(CellStyle).Text(customer.PhoneNumber ?? "-");
                                table.Cell().Element(CellStyle).Text($"{customer.TotalSpent:N2}");
                                table.Cell().Element(CellStyle).Text(customer.VisitCount);
                                table.Cell().Element(CellStyle).Text(FormatDate(customer.LastPurchaseAt));
                            }
                        });
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Generated on ");
                            text.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                        });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GeneratePendingCreditsReport(List<PendingCreditReportDTO> customers, int olderThanDays)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("Pending Credits Report").FontSize(22).Bold();
                        col.Item().Text($"Aging: {olderThanDays} days or more")
                            .FontSize(11)
                            .FontColor(Colors.Grey.Darken1);
                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    page.Content().Column(col =>
                    {
                        col.Spacing(12);
                        col.Item().Text($"Total customers: {customers.Count}").FontSize(11);

                        if (customers.Count == 0)
                        {
                            col.Item().Text("No data available for the selected period.");
                            return;
                        }

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1.4f);
                                columns.RelativeColumn(1.1f);
                                columns.RelativeColumn(1.1f);
                                columns.RelativeColumn(1.1f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyleHeader).Text("ID");
                                header.Cell().Element(CellStyleHeader).Text("Name");
                                header.Cell().Element(CellStyleHeader).Text("Email");
                                header.Cell().Element(CellStyleHeader).Text("Phone");
                                header.Cell().Element(CellStyleHeader).Text("Credit balance");
                                header.Cell().Element(CellStyleHeader).Text("Outstanding");
                                header.Cell().Element(CellStyleHeader).Text("Oldest due");
                            });

                            foreach (var customer in customers)
                            {
                                table.Cell().Element(CellStyle).Text(customer.Id);
                                table.Cell().Element(CellStyle).Text(BuildDisplayName(customer.FullName, customer.UserName));
                                table.Cell().Element(CellStyle).Text(customer.Email ?? "-");
                                table.Cell().Element(CellStyle).Text(customer.PhoneNumber ?? "-");
                                table.Cell().Element(CellStyle).Text($"{customer.CreditBalance:N2}");
                                table.Cell().Element(CellStyle).Text($"{customer.OutstandingAmount:N2}");
                                table.Cell().Element(CellStyle).Text(FormatDate(customer.OldestDueDate));
                            }
                        });
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Generated on ");
                            text.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
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

        private static string FormatDate(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("yyyy-MM-dd") : "-";
        }

        private static string BuildDisplayName(string fullName, string userName)
        {
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                return fullName;
            }

            if (!string.IsNullOrWhiteSpace(userName))
            {
                return userName;
            }

            return "-";
        }
    }
}