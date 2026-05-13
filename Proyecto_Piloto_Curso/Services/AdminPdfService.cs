using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Proyecto_Piloto_Curso.Modelos;

namespace Proyecto_Piloto_Curso.Services
{
    public class AdminPdfService
    {
        public AdminPdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        // Reporte vacío (cuando no hay datos)
        public byte[] GenerarReporteVacio(string mensaje)
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                    page.Header().Column(col =>
                    {
                        col.Item().AlignCenter().Text("SISTEMA DE CURSOS ONLINE").FontSize(14).SemiBold();
                        col.Item().AlignCenter().Text("REPORTE ADMINISTRATIVO").FontSize(12);
                        col.Item().PaddingVertical(5).LineHorizontal(1);
                    });

                    page.Content().Column(col =>
                    {
                        col.Item().PaddingTop(50).AlignCenter().Text(mensaje).FontSize(14).Bold();
                        col.Item().PaddingTop(20).AlignCenter().Text("No hay datos disponibles para mostrar");
                    });

                    page.Footer().AlignCenter().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                });
            });

            return documento.GeneratePdf();
        }

        // Reporte General de Inscripciones
        public byte[] GenerarReporteGeneralInscripciones(List<Inscripcion> todasLasInscripciones)
        {
            if (todasLasInscripciones == null || !todasLasInscripciones.Any())
            {
                return GenerarReporteVacio("No hay inscripciones registradas");
            }

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
                                header.Cell().Text("FECHA");
                            });

                            foreach (var ins in todasLasInscripciones)
                            {
                                var estudiante = ins.UsuarioNavigation?.nombre ?? "N/A";
                                var curso = ins.CursoNavigation?.titulo ?? "N/A";
                                var categoria = ins.CursoNavigation?.CategoriaNavigation?.nombre ?? "General";
                                var docente = ins.CursoNavigation?.DocenteNavigation?.UsuarioNavigation?.nombre ?? "N/A";
                                var fecha = ins.fecha_inscripcion.ToString("dd/MM/yyyy");

                                table.Cell().Text(estudiante);
                                table.Cell().Text(curso);
                                table.Cell().Text(categoria);
                                table.Cell().Text(docente);
                                table.Cell().Text(fecha);
                            }
                        });

                        col.Item().PaddingTop(20).LineHorizontal(0.5f);
                        col.Item().Text($"Total de inscripciones: {todasLasInscripciones.Count}");
                        col.Item().Text($"Fecha del reporte: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                    });
                });
            });

            return documento.GeneratePdf();
        }

        // Reporte General de Cursos (SIN columna ID)
        public byte[] GenerarReporteGeneralCursos(List<Curso> cursos)
        {
            if (cursos == null || !cursos.Any())
            {
                return GenerarReporteVacio("No hay cursos registrados");
            }

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
                        col.Item().AlignCenter().Text("REPORTE GENERAL DE CURSOS").FontSize(12);
                        col.Item().PaddingVertical(5).LineHorizontal(1);
                    });

                    page.Content().Column(col =>
                    {
                        col.Item().PaddingTop(10).Text("LISTADO DE CURSOS").SemiBold();
                        col.Item().LineHorizontal(0.5f);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(5);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1.5f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("CURSO");
                                header.Cell().Text("CATEGORIA");
                                header.Cell().Text("PRECIO");
                            });

                            foreach (var curso in cursos)
                            {
                                var titulo = curso.titulo ?? "N/A";
                                var categoria = curso.CategoriaNavigation?.nombre ?? "N/A";
                                var precio = curso.precio == 0 ? "Gratis" : $"${curso.precio:F2}";

                                table.Cell().Text(titulo);
                                table.Cell().Text(categoria);
                                table.Cell().Text(precio);
                            }
                        });

                        col.Item().PaddingTop(20).LineHorizontal(0.5f);
                        col.Item().Text($"Total de cursos: {cursos.Count}");
                        col.Item().Text($"Fecha del reporte: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                    });
                });
            });

            return documento.GeneratePdf();
        }

        // Reporte General de Usuarios (ESTUDIANTES y DOCENTES separados)
        public byte[] GenerarReporteGeneralUsuarios(List<Usuario> usuarios, List<Docente> docentes)
        {
            if (usuarios == null || !usuarios.Any())
            {
                return GenerarReporteVacio("No hay usuarios registrados");
            }

            var estudiantes = usuarios.Where(u => u.rol == "estudiante").ToList();
            var listaDocentes = usuarios.Where(u => u.rol == "docente").ToList();
            var admins = usuarios.Where(u => u.rol == "admin").ToList();

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
                        col.Item().AlignCenter().Text("REPORTE GENERAL DE USUARIOS").FontSize(12);
                        col.Item().PaddingVertical(5).LineHorizontal(1);
                    });

                    page.Content().Column(col =>
                    {
                        // Estadísticas
                        col.Item().PaddingTop(10).Text("ESTADISTICAS DE USUARIOS").SemiBold();
                        col.Item().LineHorizontal(0.5f);
                        col.Item().PaddingVertical(5).Column(statsCol =>
                        {
                            statsCol.Item().Text($"Total de estudiantes: {estudiantes.Count}");
                            statsCol.Item().Text($"Total de docentes: {listaDocentes.Count}");
                            statsCol.Item().Text($"Total de administradores: {admins.Count}");
                            statsCol.Item().Text($"Total general: {usuarios.Count}");
                        });

                        // Tabla de Estudiantes
                        if (estudiantes.Any())
                        {
                            col.Item().PaddingTop(15).Text("LISTA DE ESTUDIANTES").SemiBold();
                            col.Item().LineHorizontal(0.5f);

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Text("NOMBRE");
                                    header.Cell().Text("CORREO");
                                    header.Cell().Text("FECHA REGISTRO");
                                });

                                foreach (var estudiante in estudiantes)
                                {
                                    var nombre = estudiante.nombre ?? "N/A";
                                    var correo = estudiante.correo ?? "N/A";
                                    var fecha = estudiante.fecha_creacion.ToString("dd/MM/yyyy");

                                    table.Cell().Text(nombre);
                                    table.Cell().Text(correo);
                                    table.Cell().Text(fecha);
                                }
                            });
                        }

                        // Tabla de Docentes (con área)
                        if (docentes != null && docentes.Any())
                        {
                            col.Item().PaddingTop(15).Text("LISTA DE DOCENTES").SemiBold();
                            col.Item().LineHorizontal(0.5f);

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(2.5f);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Text("NOMBRE");
                                    header.Cell().Text("CORREO");
                                    header.Cell().Text("AREA");
                                });

                                foreach (var docente in docentes)
                                {
                                    var nombre = docente.UsuarioNavigation?.nombre ?? "N/A";
                                    var correo = docente.UsuarioNavigation?.correo ?? "N/A";
                                    var area = docente.area ?? "N/A";

                                    table.Cell().Text(nombre);
                                    table.Cell().Text(correo);
                                    table.Cell().Text(area);
                                }
                            });
                        }

                        col.Item().PaddingTop(20).LineHorizontal(0.5f);
                        col.Item().Text($"Fecha del reporte: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                    });
                });
            });

            return documento.GeneratePdf();
        }
    }
}