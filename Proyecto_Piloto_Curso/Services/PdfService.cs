using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Proyecto_Piloto_Curso.Modelos;

namespace Proyecto_Piloto_Curso.Services
{
    public class PdfService
    {
        public byte[] GenerarReporteAprendizaje(Usuario estudiante, List<Inscripcion> inscripciones)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Column(col =>
                    {
                        col.Item().AlignCenter().Text("SISTEMA DE CURSOS ONLINE").FontSize(14).SemiBold();
                        col.Item().AlignCenter().Text("REPORTE ESTUDIANTIL").FontSize(12);
                        col.Item().PaddingVertical(5).LineHorizontal(1);
                    });

                    page.Content().Column(col =>
                    {
                        col.Item().PaddingTop(10).Text("DATOS DEL ESTUDIANTE").SemiBold();
                        col.Item().LineHorizontal(0.5f);
                        col.Item().PaddingVertical(5).Column(userCol =>
                        {
                            userCol.Item().Text($"Nombre: {estudiante.nombre}");
                            userCol.Item().Text($"Usuario: {estudiante.username}");
                            userCol.Item().Text($"Correo: {estudiante.correo}");
                        });

                        col.Item().PaddingTop(15).Text("CURSOS INSCRITOS").SemiBold();
                        col.Item().LineHorizontal(0.5f);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(5);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(3);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("CURSO");
                                header.Cell().Text("CATEGORIA");
                                header.Cell().Text("DOCENTE");
                            });

                            foreach (var ins in inscripciones)
                            {
                                table.Cell().Text(ins.CursoNavigation?.titulo ?? "N/A");
                                table.Cell().Text(ins.CursoNavigation?.CategoriaNavigation?.nombre ?? "General");
                                table.Cell().Text(ins.CursoNavigation?.DocenteNavigation?.UsuarioNavigation?.nombre ?? "N/A");
                            }
                        });

                        col.Item().PaddingTop(20).LineHorizontal(0.5f);
                        col.Item().Text($"Total cursos inscritos: {inscripciones.Count}");
                        col.Item().Text($"Fecha del reporte: {DateTime.Now:dd/MM/yyyy}");
                    });
                });
            });

            return documento.GeneratePdf();
        }

        public byte[] GenerarReporteGeneralInscripciones(List<Inscripcion> todasLasInscripciones)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Column(col =>
                    {
                        col.Item().AlignCenter().Text("SISTEMA DE CURSOS ONLINE").FontSize(14).SemiBold();
                        col.Item().AlignCenter().Text("REPORTE GENERAL DE INSCRIPCIONES").FontSize(12);
                        col.Item().PaddingVertical(5).LineHorizontal(1);
                    });

                    page.Content().Column(col =>
                    {
                        col.Item().PaddingTop(10).Text("RESUMEN DE INSCRIPCIONES").SemiBold();
                        col.Item().LineHorizontal(0.5f);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2.5f);
                                columns.RelativeColumn(2.5f);
                                columns.RelativeColumn(1.5f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("ESTUDIANTE");
                                header.Cell().Text("CURSO");
                                header.Cell().Text("CATEGORIA");
                                header.Cell().Text("DOCENTE");
                                header.Cell().Text("FECHA INSCRIPCION");
                            });

                            foreach (var ins in todasLasInscripciones)
                            {
                                table.Cell().Text(ins.UsuarioNavigation?.nombre ?? "N/A");
                                table.Cell().Text(ins.CursoNavigation?.titulo ?? "N/A");
                                table.Cell().Text(ins.CursoNavigation?.CategoriaNavigation?.nombre ?? "General");
                                table.Cell().Text(ins.CursoNavigation?.DocenteNavigation?.UsuarioNavigation?.nombre ?? "N/A");
                                table.Cell().Text(ins.fecha_inscripcion.ToString("dd/MM/yyyy"));
                            }
                        });

                        col.Item().PaddingTop(20).LineHorizontal(0.5f);
                        col.Item().Text($"Total de inscripciones: {todasLasInscripciones.Count}");
                        col.Item().Text($"Fecha del reporte: {DateTime.Now:dd/MM/yyyy}");
                    });
                });
            });

            return documento.GeneratePdf();
        }
    }
}