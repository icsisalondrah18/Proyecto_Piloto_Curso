create database piloto_Cursos
use piloto_Cursos
go


CREATE TABLE usuarios (
  idUsuarios INT PRIMARY KEY IDENTITY(1,1), -- IDENTITY para que sea autoincremental
  username VARCHAR(50) NOT NULL UNIQUE, -- Este será el campo para el login
  nombre VARCHAR(100) NOT NULL,
  correo VARCHAR(50) NOT NULL,
  contrasena VARCHAR(100) NOT NULL,
  rol VARCHAR(20) CHECK (rol IN ('estudiante','docente','admin')),
  fecha_creacion DATETIME DEFAULT GETDATE()
);

CREATE TABLE categorias (
  idCat INT PRIMARY KEY,
  nombre VARCHAR(100)
);

CREATE TABLE descuentos (
  idDescuentos INT PRIMARY KEY,
  codigo VARCHAR(50),
  tipo VARCHAR(20) CHECK (tipo IN ('porcentaje','fijo')),
  valor DECIMAL(10,2),
  max_usos INT,
  usos_actuales INT DEFAULT 0,
  fecha_inicio DATETIME,
  fecha_fin DATETIME,
  activo BIT DEFAULT 1,
  fecha_creacion DATETIME DEFAULT GETDATE()
);

CREATE TABLE docentes (
  idDocente INT PRIMARY KEY,
  area VARCHAR(100), 
  biografia TEXT,
  CONSTRAINT fk_docente_usuario FOREIGN KEY (idDocente) REFERENCES usuarios(idUsuarios)
);

CREATE TABLE tarjetas (
  idTarjeta INT PRIMARY KEY,
  usuario_id INT,
  numero_tarjeta VARCHAR(20),
  nombre_titular VARCHAR(100),
  fecha_expiracion VARCHAR(7),
  CONSTRAINT fk_tarjeta_usuario FOREIGN KEY (usuario_id) REFERENCES usuarios(idUsuarios)
);

CREATE TABLE usuario_descuentos (
  idUsuarioDesc INT PRIMARY KEY,
  usuario_id INT,
  descuento_id INT,
  usado BIT DEFAULT 0,
  CONSTRAINT fk_usuario_descuento_usuario FOREIGN KEY (usuario_id) REFERENCES usuarios(idUsuarios),
  CONSTRAINT fk_usuario_descuento_descuento FOREIGN KEY (descuento_id) REFERENCES descuentos(idDescuentos)
);

CREATE TABLE cursos (
  idCursos INT PRIMARY KEY,
  titulo VARCHAR(255),
  descripcion TEXT,
  precio DECIMAL(10,2),
  docenteId INT,
  categoria_id INT,
  fecha_creacion DATETIME DEFAULT GETDATE(),
  CONSTRAINT fk_curso_docente FOREIGN KEY (docenteId) REFERENCES docentes(idDocente),
  CONSTRAINT fk_curso_categoria FOREIGN KEY (categoria_id) REFERENCES categorias(idCat)
);

CREATE TABLE objetivos (
  idObjetivo INT PRIMARY KEY,
  curso_id INT,
  descripcion TEXT,
  CONSTRAINT fk_objetivo_curso FOREIGN KEY (curso_id) REFERENCES cursos(idCursos)
);

CREATE TABLE modulosCurso (
  idModulo INT PRIMARY KEY IDENTITY(1,1),
  cursoId INT,
  titulo VARCHAR(255),
  tipo_contenido VARCHAR(20) CHECK (tipo_contenido IN ('video','texto','quiz')),
  url_video TEXT,
  duracion INT,
  orden INT,
  CONSTRAINT fk_modulo_curso FOREIGN KEY (cursoId) REFERENCES cursos(idCursos)
);

CREATE TABLE inscripciones (
  idInscripciones INT PRIMARY KEY,
  usuario_id INT,
  curso_id INT,
  progreso DECIMAL(5,2) DEFAULT 0,
  fecha_inscripcion DATETIME DEFAULT GETDATE(),
  CONSTRAINT fk_inscripcion_usuario FOREIGN KEY (usuario_id) REFERENCES usuarios(idUsuarios),
  CONSTRAINT fk_inscripcion_curso FOREIGN KEY (curso_id) REFERENCES cursos(idCursos)
);

CREATE TABLE resenas (
  idResenas INT PRIMARY KEY,
  usuario_id INT,
  curso_id INT,
  calificacion INT CHECK (calificacion BETWEEN 1 AND 5),
  comentario TEXT,
  fecha_creacion DATETIME DEFAULT GETDATE(),
  CONSTRAINT fk_resena_usuario FOREIGN KEY (usuario_id) REFERENCES usuarios(idUsuarios),
  CONSTRAINT fk_resena_curso FOREIGN KEY (curso_id) REFERENCES cursos(idCursos)
);

CREATE TABLE pagos (
  idPagos INT PRIMARY KEY,
  usuario_id INT,
  curso_id INT,
  tarjeta_id INT,
  precio_original DECIMAL(10,2),
  descuento_id INT,
  precio_final DECIMAL(10,2),
  fecha_creacion DATETIME DEFAULT GETDATE(),
  CONSTRAINT fk_pago_usuario FOREIGN KEY (usuario_id) REFERENCES usuarios(idUsuarios),
  CONSTRAINT fk_pago_curso FOREIGN KEY (curso_id) REFERENCES cursos(idCursos),
  CONSTRAINT fk_pago_descuento FOREIGN KEY (descuento_id) REFERENCES descuentos(idDescuentos),
  CONSTRAINT fk_pago_tarjeta FOREIGN KEY (tarjeta_id) REFERENCES tarjetas(idTarjeta)
);

CREATE TABLE curso_descuentos (
  idCurDesc INT PRIMARY KEY,
  curso_id INT,
  descuento_id INT,
  CONSTRAINT fk_curso_descuento_curso FOREIGN KEY (curso_id) REFERENCES cursos(idCursos),
  CONSTRAINT fk_curso_descuento_descuento FOREIGN KEY (descuento_id) REFERENCES descuentos(idDescuentos)
);

CREATE TABLE certificados (
  idCertificado INT PRIMARY KEY,
  usuario_id INT,
  curso_id INT,
  fecha_emision DATE,
  url_certificado TEXT,
  CONSTRAINT fk_certificado_usuario FOREIGN KEY (usuario_id) REFERENCES usuarios(idUsuarios),
  CONSTRAINT fk_certificado_curso FOREIGN KEY (curso_id) REFERENCES cursos(idCursos)
);


----ALTER

ALTER TABLE tarjetas ADD tipo_tarjeta VARCHAR(20) NULL;

ALTER TABLE tarjetas ADD cvv VARCHAR(10) NULL; -- Agregar columna CVV

----INSERCIONES

INSERT INTO usuarios (username, nombre, correo, contrasena, rol)
VALUES 
('admin','Carlos Mendoza','carlosmendoza@gmail.com','482915','admin');

INSERT INTO usuarios (username, nombre, correo, contrasena, rol)
VALUES
('prog_javier','Javier López','javierlopez@gmail.com','174620','docente'),
('prog_maria','María Torres','mariatorres@gmail.com','583941','docente'),

('design_luis','Luis Gaitán','luisgaitan@gmail.com','260714','docente'),
('design_ana','Ana Morales','anamorales@gmail.com','931485','docente'),

('redes_kevin','Kevin Rocha','kevinrocha@gmail.com','715392','docente'),
('redes_sofia','Sofía Herrera','sofiaherrera@gmail.com','408627','docente');

INSERT INTO docentes (idDocente, area, biografia)
VALUES
(2,'Programación','Especialista en desarrollo web y bases de datos'),
(3,'Programación','Docente enfocada en programación orientada a objetos'),

(4,'Diseño Gráfico','Experto en diseño digital y branding'),
(5,'Diseño Gráfico','Especialista en ilustración y edición multimedia'),

(6,'Redes','Administrador de redes y ciberseguridad'),
(7,'Redes','Especialista en infraestructura y telecomunicaciones');

INSERT INTO usuarios (username, nombre, correo, contrasena, rol)
VALUES
('juanmartinez','Juan Martínez','juanmartinez@gmail.com','100001','estudiante'),
('marialopez','María López','marialopez@gmail.com','100002','estudiante'),
('carloshernandez','Carlos Hernández','carloshernandez@gmail.com','100003','estudiante'),
('anagarcia','Ana García','anagarcia@gmail.com','100004','estudiante'),
('luisrodriguez','Luis Rodríguez','luisrodriguez@gmail.com','100005','estudiante'),
('sofiaramirez','Sofía Ramírez','sofiaramirez@gmail.com','100006','estudiante'),
('joseperez','José Pérez','joseperez@gmail.com','100007','estudiante'),
('camilatorres','Camila Torres','camilatorres@gmail.com','100008','estudiante'),
('miguelsanchez','Miguel Sánchez','miguelsanchez@gmail.com','100009','estudiante'),
('valentinaflores','Valentina Flores','valentinaflores@gmail.com','100010','estudiante'),

('andresmorales','Andrés Morales','andresmorales@gmail.com','100011','estudiante'),
('fernandacastro','Fernanda Castro','fernandacastro@gmail.com','100012','estudiante'),
('diegoruiz','Diego Ruiz','diegoruiz@gmail.com','100013','estudiante'),
('danielamendoza','Daniela Mendoza','danielamendoza@gmail.com','100014','estudiante'),
('javiernavarro','Javier Navarro','javiernavarro@gmail.com','100015','estudiante'),
('paulaherrera','Paula Herrera','paulaherrera@gmail.com','100016','estudiante'),
('ricardogomez','Ricardo Gómez','ricardogomez@gmail.com','100017','estudiante'),
('gabrielasilva','Gabriela Silva','gabrielasilva@gmail.com','100018','estudiante'),
('alejandrovega','Alejandro Vega','alejandrovega@gmail.com','100019','estudiante'),
('nataliacruz','Natalia Cruz','nataliacruz@gmail.com','100020','estudiante'),

('kevinromero','Kevin Romero','kevinromero@gmail.com','100021','estudiante'),
('lauramedina','Laura Medina','lauramedina@gmail.com','100022','estudiante'),
('brayanortiz','Brayan Ortiz','brayanortiz@gmail.com','100023','estudiante'),
('melissarojas','Melissa Rojas','melissarojas@gmail.com','100024','estudiante'),
('cristianleon','Cristian León','cristianleon@gmail.com','100025','estudiante'),
('dianavargas','Diana Vargas','dianavargas@gmail.com','100026','estudiante'),
('angelcastillo','Ángel Castillo','angelcastillo@gmail.com','100027','estudiante'),
('kimberlyreyes','Kimberly Reyes','kimberlyreyes@gmail.com','100028','estudiante'),
('leonardosalinas','Leonardo Salinas','leonardosalinas@gmail.com','100029','estudiante'),
('ashleyduarte','Ashley Duarte','ashleyduarte@gmail.com','100030','estudiante');

INSERT INTO categorias (idCat, nombre)
VALUES
(1,'Programación'),
(2,'Diseño Gráfico'),
(3,'Redes');

INSERT INTO cursos 
(idCursos, titulo, descripcion, precio, docenteId, categoria_id)
VALUES

(1,'Fundamentos de Programación','Introducción a la lógica de programación',0.00,2,1),
(2,'Programación en C#','Desarrollo de aplicaciones con C#',0.00,2,1),
(3,'Bases de Datos SQL Server','Administración de bases de datos relacionales',0.00,3,1),
(4,'Desarrollo Web ASP.NET','Creación de aplicaciones web dinámicas',0.00,3,1),
(5,'Programación Orientada a Objetos','Principios de POO y buenas prácticas',0.00,2,1),
(6,'Java para Principiantes','Introducción al lenguaje Java',0.00,2,1),
(7,'Python Básico','Automatización y programación básica',0.00,3,1),
(8,'Desarrollo Frontend','HTML, CSS y JavaScript moderno',0.00,3,1),
(9,'APIs REST','Creación de servicios web RESTful',0.00,2,1),
(10,'Git y GitHub','Control de versiones profesional',0.00,3,1),

(11,'Diseño Gráfico Básico','Principios fundamentales del diseño visual',0.00,4,2),
(12,'Adobe Photoshop','Edición profesional de imágenes',0.00,4,2),
(13,'Adobe Illustrator','Diseño vectorial profesional',0.00,5,2),
(14,'Diseño de Logos','Creación de identidad visual',0.00,5,2),
(15,'Diseño Publicitario','Diseño de contenido para marketing',0.00,4,2),
(16,'Edición de Video','Producción y edición multimedia',0.00,4,2),
(17,'Diseño UI/UX','Experiencia e interfaz de usuario',0.00,5,2),
(18,'Canva Profesional','Diseño rápido para redes sociales',0.00,5,2),
(19,'Animación Digital','Introducción a motion graphics',0.00,4,2),
(20,'Branding Empresarial','Construcción de marcas modernas',0.00,5,2),

(21,'Fundamentos de Redes','Conceptos esenciales de networking',0.00,6,3),
(22,'Cisco CCNA Básico','Configuración inicial de redes Cisco',0.00,6,3),
(23,'Ciberseguridad','Protección de redes y sistemas',0.00,7,3),
(24,'Administración de Servidores','Gestión de servidores empresariales',0.00,7,3),
(25,'Redes Inalámbricas','Configuración de redes WiFi',0.00,6,3),
(26,'Linux para Redes','Administración Linux orientada a redes',0.00,6,3),
(27,'Cableado Estructurado','Infraestructura física de redes',0.00,7,3),
(28,'Firewall y Seguridad','Implementación de seguridad perimetral',0.00,7,3),
(29,'Virtualización','Uso de máquinas virtuales y VMware',0.00,6,3),
(30,'Cloud Computing','Servicios y tecnologías en la nube',0.00,7,3);

----
-----NUEVOOOOOO
----

-- CATEGORÍA 1: PROGRAMACIÓN 


-- Curso 1: Fundamentos de Programación
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(1, 'Introducción a la programación', 'texto', NULL, 15, 1),
(1, 'Variables y tipos de datos', 'texto', NULL, 20, 2),
(1, 'Estructuras condicionales', 'texto', NULL, 25, 3),
(1, 'Bucles y ciclos', 'texto', NULL, 22, 4),
(1, 'Funciones y procedimientos', 'texto', NULL, 18, 5),
(1, 'Evaluación final', 'quiz', NULL, 30, 6);

-- Curso 2: Programación en C#
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(2, 'Introducción a C#', 'texto', NULL, 15, 1),
(2, 'Sintaxis básica', 'texto', NULL, 20, 2),
(2, 'Programación orientada a objetos', 'texto', NULL, 30, 3),
(2, 'Manejo de excepciones', 'texto', NULL, 18, 4),
(2, 'LINQ y colecciones', 'texto', NULL, 25, 5),
(2, 'Proyecto final', 'quiz', NULL, 40, 6);

-- Curso 3: Bases de Datos SQL Server
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(3, 'Introducción a las BD', 'texto', NULL, 15, 1),
(3, 'Modelado de datos', 'texto', NULL, 25, 2),
(3, 'Consultas SELECT', 'texto', NULL, 22, 3),
(3, 'JOIN y subconsultas', 'texto', NULL, 28, 4),
(3, 'Procedimientos almacenados', 'texto', NULL, 20, 5),
(3, 'Evaluación final', 'quiz', NULL, 35, 6);

-- Curso 4: Desarrollo Web ASP.NET
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(4, 'Introducción a ASP.NET', 'texto', NULL, 15, 1),
(4, 'MVC - Modelo Vista Controlador', 'texto', NULL, 30, 2),
(4, 'Razor Pages', 'texto', NULL, 25, 3),
(4, 'Entity Framework Core', 'texto', NULL, 35, 4),
(4, 'Autenticación y autorización', 'texto', NULL, 28, 5),
(4, 'Proyecto final', 'quiz', NULL, 40, 6);

-- Curso 5: Programación Orientada a Objetos
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(5, 'Principios de POO', 'texto', NULL, 20, 1),
(5, 'Clases y objetos', 'texto', NULL, 22, 2),
(5, 'Herencia', 'texto', NULL, 25, 3),
(5, 'Polimorfismo', 'texto', NULL, 20, 4),
(5, 'Encapsulamiento', 'texto', NULL, 18, 5),
(5, 'Evaluación final', 'quiz', NULL, 30, 6);

-- Curso 6: Java para Principiantes
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(6, 'Instalación y setup', 'texto', NULL, 10, 1),
(6, 'Primeros pasos en Java', 'texto', NULL, 20, 2),
(6, 'Tipos de datos y operadores', 'texto', NULL, 25, 3),
(6, 'Estructuras de control', 'texto', NULL, 22, 4),
(6, 'Arreglos y matrices', 'texto', NULL, 18, 5),
(6, 'Programación orientada a objetos', 'quiz', NULL, 30, 6);

-- Curso 7: Python Básico
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(7, 'Introducción a Python', 'texto', NULL, 15, 1),
(7, 'Variables y tipos', 'texto', NULL, 18, 2),
(7, 'Listas y tuplas', 'texto', NULL, 22, 3),
(7, 'Diccionarios y conjuntos', 'texto', NULL, 20, 4),
(7, 'Funciones en Python', 'texto', NULL, 25, 5),
(7, 'Evaluación final', 'quiz', NULL, 35, 6);

-- Curso 8: Desarrollo Frontend
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(8, 'Introducción a HTML', 'texto', NULL, 15, 1),
(8, 'CSS3 y estilos', 'texto', NULL, 25, 2),
(8, 'JavaScript básico', 'texto', NULL, 30, 3),
(8, 'DOM y eventos', 'texto', NULL, 22, 4),
(8, 'Responsive Design', 'texto', NULL, 20, 5),
(8, 'Proyecto final', 'quiz', NULL, 40, 6);

-- Curso 9: APIs REST
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(9, '¿Qué es una API?', 'texto', NULL, 15, 1),
(9, 'HTTP y métodos REST', 'texto', NULL, 20, 2),
(9, 'JSON y XML', 'texto', NULL, 18, 3),
(9, 'Autenticación y tokens', 'texto', NULL, 25, 4),
(9, 'Documentación de APIs', 'texto', NULL, 15, 5),
(9, 'Evaluación final', 'quiz', NULL, 30, 6);

-- Curso 10: Git y GitHub
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(10, 'Introducción a Git', 'texto', NULL, 15, 1),
(10, 'Comandos básicos', 'texto', NULL, 20, 2),
(10, 'Ramas y fusiones', 'texto', NULL, 25, 3),
(10, 'GitHub y colaboración', 'texto', NULL, 22, 4),
(10, 'Flujo de trabajo profesional', 'texto', NULL, 18, 5),
(10, 'Proyecto final', 'quiz', NULL, 30, 6);


-- CATEGORÍA 2: DISEÑO GRÁFICO 


-- Curso 11: Diseño Gráfico Básico
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(11, 'Principios del diseño', 'texto', NULL, 15, 1),
(11, 'Teoría del color', 'texto', NULL, 20, 2),
(11, 'Tipografía', 'texto', NULL, 18, 3),
(11, 'Composición y layout', 'texto', NULL, 22, 4),
(11, 'Herramientas de diseño', 'texto', NULL, 15, 5),
(11, 'Evaluación final', 'quiz', NULL, 25, 6);

-- Curso 12: Adobe Photoshop
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(12, 'Interfaz de Photoshop', 'texto', NULL, 15, 1),
(12, 'Capas y máscaras', 'texto', NULL, 25, 2),
(12, 'Ajustes de color', 'texto', NULL, 20, 3),
(12, 'Retoque fotográfico', 'texto', NULL, 28, 4),
(12, 'Filtros y efectos', 'texto', NULL, 18, 5),
(12, 'Proyecto final', 'quiz', NULL, 30, 6);

-- Curso 13: Adobe Illustrator
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(13, 'Introducción a Illustrator', 'texto', NULL, 15, 1),
(13, 'Herramientas de dibujo', 'texto', NULL, 22, 2),
(13, 'Trazados y curvas', 'texto', NULL, 20, 3),
(13, 'Color y degradados', 'texto', NULL, 18, 4),
(13, 'Diseño de logotipos', 'texto', NULL, 25, 5),
(13, 'Evaluación final', 'quiz', NULL, 30, 6);

-- Curso 14: Diseño de Logos
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(14, 'Tipos de logotipos', 'texto', NULL, 15, 1),
(14, 'Proceso creativo', 'texto', NULL, 20, 2),
(14, 'Bocetaje y conceptualización', 'texto', NULL, 22, 3),
(14, 'Digitalización', 'texto', NULL, 18, 4),
(14, 'Presentación al cliente', 'texto', NULL, 15, 5),
(14, 'Proyecto final', 'quiz', NULL, 25, 6);

-- Curso 15: Diseño Publicitario
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(15, 'Introducción a la publicidad', 'texto', NULL, 15, 1),
(15, 'Psicología del color', 'texto', NULL, 20, 2),
(15, 'Diseño de anuncios', 'texto', NULL, 22, 3),
(15, 'Publicidad digital', 'texto', NULL, 18, 4),
(15, 'Campañas publicitarias', 'texto', NULL, 25, 5),
(15, 'Evaluación final', 'quiz', NULL, 30, 6);

-- Curso 16: Edición de Video
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(16, 'Introducción a la edición', 'texto', NULL, 15, 1),
(16, 'Corte y montaje', 'texto', NULL, 20, 2),
(16, 'Transiciones y efectos', 'texto', NULL, 18, 3),
(16, 'Audio y titulación', 'texto', NULL, 22, 4),
(16, 'Exportación y formatos', 'texto', NULL, 15, 5),
(16, 'Proyecto final', 'quiz', NULL, 30, 6);

-- Curso 17: Diseño UI/UX
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(17, 'Introducción a UI/UX', 'texto', NULL, 15, 1),
(17, 'Investigación de usuarios', 'texto', NULL, 20, 2),
(17, 'Wireframes y prototipos', 'texto', NULL, 22, 3),
(17, 'Diseño de interfaces', 'texto', NULL, 25, 4),
(17, 'Pruebas de usabilidad', 'texto', NULL, 18, 5),
(17, 'Evaluación final', 'quiz', NULL, 30, 6);

-- Curso 18: Canva Profesional
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(18, 'Introducción a Canva', 'texto', NULL, 10, 1),
(18, 'Herramientas básicas', 'texto', NULL, 15, 2),
(18, 'Plantillas profesionales', 'texto', NULL, 12, 3),
(18, 'Diseño para redes sociales', 'texto', NULL, 18, 4),
(18, 'Branding con Canva', 'texto', NULL, 15, 5),
(18, 'Proyecto final', 'quiz', NULL, 20, 6);

-- Curso 19: Animación Digital
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(19, 'Principios de animación', 'texto', NULL, 15, 1),
(19, 'Storyboard y planificación', 'texto', NULL, 20, 2),
(19, 'Animación 2D', 'texto', NULL, 25, 3),
(19, 'Motion graphics', 'texto', NULL, 22, 4),
(19, 'Exportación de animaciones', 'texto', NULL, 15, 5),
(19, 'Evaluación final', 'quiz', NULL, 30, 6);

-- Curso 20: Branding Empresarial
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(20, 'Introducción al branding', 'texto', NULL, 15, 1),
(20, 'Identidad corporativa', 'texto', NULL, 20, 2),
(20, 'Manual de marca', 'texto', NULL, 22, 3),
(20, 'Estrategia de marca', 'texto', NULL, 18, 4),
(20, 'Posicionamiento', 'texto', NULL, 15, 5),
(20, 'Proyecto final', 'quiz', NULL, 25, 6);


-- CATEGORÍA 3: REDES 


-- Curso 21: Fundamentos de Redes
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(21, 'Introducción a redes', 'texto', NULL, 15, 1),
(21, 'Topologías de red', 'texto', NULL, 20, 2),
(21, 'Modelo OSI', 'texto', NULL, 25, 3),
(21, 'Direccionamiento IP', 'texto', NULL, 22, 4),
(21, 'Subneteo', 'texto', NULL, 30, 5),
(21, 'Evaluación final', 'quiz', NULL, 35, 6);

-- Curso 22: Cisco CCNA Básico
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(22, 'Introducción a CCNA', 'texto', NULL, 15, 1),
(22, 'Configuración básica', 'texto', NULL, 25, 2),
(22, 'Enrutamiento estático', 'texto', NULL, 22, 3),
(22, 'Enrutamiento dinámico', 'texto', NULL, 28, 4),
(22, 'VLANs y trunking', 'texto', NULL, 20, 5),
(22, 'Proyecto final', 'quiz', NULL, 30, 6);

-- Curso 23: Ciberseguridad
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(23, 'Introducción a ciberseguridad', 'texto', NULL, 15, 1),
(23, 'Tipos de amenazas', 'texto', NULL, 22, 2),
(23, 'Criptografía básica', 'texto', NULL, 20, 3),
(23, 'Firewalls y antivirus', 'texto', NULL, 18, 4),
(23, 'Buenas prácticas', 'texto', NULL, 15, 5),
(23, 'Evaluación final', 'quiz', NULL, 25, 6);

-- Curso 24: Administración de Servidores
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(24, 'Introducción a servidores', 'texto', NULL, 15, 1),
(24, 'Windows Server', 'texto', NULL, 25, 2),
(24, 'Linux Server', 'texto', NULL, 22, 3),
(24, 'Servicios de red', 'texto', NULL, 20, 4),
(24, 'Virtualización', 'texto', NULL, 18, 5),
(24, 'Proyecto final', 'quiz', NULL, 30, 6);

-- Curso 25: Redes Inalámbricas
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(25, 'Introducción a Wi-Fi', 'texto', NULL, 15, 1),
(25, 'Estándares 802.11', 'texto', NULL, 20, 2),
(25, 'Configuración de routers', 'texto', NULL, 22, 3),
(25, 'Seguridad inalámbrica', 'texto', NULL, 18, 4),
(25, 'Antenas y cobertura', 'texto', NULL, 15, 5),
(25, 'Evaluación final', 'quiz', NULL, 25, 6);

-- Curso 26: Linux para Redes
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(26, 'Introducción a Linux', 'texto', NULL, 15, 1),
(26, 'Comandos básicos', 'texto', NULL, 20, 2),
(26, 'Administración del sistema', 'texto', NULL, 22, 3),
(26, 'Servicios en Linux', 'texto', NULL, 25, 4),
(26, 'Firewall iptables', 'texto', NULL, 18, 5),
(26, 'Proyecto final', 'quiz', NULL, 30, 6);

-- Curso 27: Cableado Estructurado
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(27, 'Introducción al cableado', 'texto', NULL, 15, 1),
(27, 'Tipos de cables', 'texto', NULL, 18, 2),
(27, 'Conectores y herramientas', 'texto', NULL, 20, 3),
(27, 'Normativas y estándares', 'texto', NULL, 15, 4),
(27, 'Certificación de cableado', 'texto', NULL, 12, 5),
(27, 'Evaluación final', 'quiz', NULL, 20, 6);

-- Curso 28: Firewall y Seguridad
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(28, 'Introducción a firewalls', 'texto', NULL, 15, 1),
(28, 'Tipos de firewalls', 'texto', NULL, 18, 2),
(28, 'Reglas y políticas', 'texto', NULL, 22, 3),
(28, 'Configuración básica', 'texto', NULL, 20, 4),
(28, 'Monitoreo de logs', 'texto', NULL, 15, 5),
(28, 'Proyecto final', 'quiz', NULL, 25, 6);

-- Curso 29: Virtualización
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(29, 'Introducción a virtualización', 'texto', NULL, 15, 1),
(29, 'Hypervisores', 'texto', NULL, 18, 2),
(29, 'VMware básico', 'texto', NULL, 22, 3),
(29, 'VirtualBox', 'texto', NULL, 20, 4),
(29, 'Contenedores Docker', 'texto', NULL, 25, 5),
(29, 'Evaluación final', 'quiz', NULL, 30, 6);

-- Curso 30: Cloud Computing
INSERT INTO modulosCurso (cursoId, titulo, tipo_contenido, url_video, duracion, orden) VALUES
(30, 'Introducción a Cloud', 'texto', NULL, 15, 1),
(30, 'IaaS, PaaS, SaaS', 'texto', NULL, 20, 2),
(30, 'AWS básico', 'texto', NULL, 25, 3),
(30, 'Azure básico', 'texto', NULL, 22, 4),
(30, 'Google Cloud', 'texto', NULL, 18, 5),
(30, 'Proyecto final', 'quiz', NULL, 30, 6);

USE piloto_Cursos;
GO
************************************

-----OBJETIVOS(OJO)
************************************

-- CATEGORÍA 1: PROGRAMACIÓN 


-- Curso 1: Fundamentos de Programación
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(1, 1, 'Comprender los conceptos básicos de programación'),
(2, 1, 'Identificar y utilizar variables y tipos de datos'),
(3, 1, 'Aplicar estructuras condicionales en problemas cotidianos'),
(4, 1, 'Desarrollar algoritmos utilizando bucles y ciclos'),
(5, 1, 'Crear funciones y procedimientos reutilizables');

-- Curso 2: Programación en C#
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(6, 2, 'Dominar la sintaxis básica de C#'),
(7, 2, 'Aplicar principios de Programación Orientada a Objetos'),
(8, 2, 'Implementar manejo de excepciones'),
(9, 2, 'Utilizar LINQ para consultas de datos'),
(10, 2, 'Desarrollar una aplicación completa en C#');

-- Curso 3: Bases de Datos SQL Server
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(11, 3, 'Comprender el modelo relacional de bases de datos'),
(12, 3, 'Diseñar diagramas entidad-relación'),
(13, 3, 'Escribir consultas SQL complejas con JOINs'),
(14, 3, 'Crear procedimientos almacenados'),
(15, 3, 'Optimizar consultas para mejor rendimiento');

-- Curso 4: Desarrollo Web ASP.NET
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(16, 4, 'Configurar un proyecto ASP.NET Core'),
(17, 4, 'Implementar el patrón MVC'),
(18, 4, 'Crear páginas dinámicas con Razor'),
(19, 4, 'Integrar Entity Framework Core'),
(20, 4, 'Publicar aplicaciones web en producción');

-- Curso 5: Programación Orientada a Objetos
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(21, 5, 'Entender los 4 pilares de POO'),
(22, 5, 'Diseñar clases y objetos'),
(23, 5, 'Implementar herencia y polimorfismo'),
(24, 5, 'Aplicar encapsulamiento correctamente'),
(25, 5, 'Utilizar interfaces y abstracción');

-- Curso 6: Java para Principiantes
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(26, 6, 'Configurar el entorno de desarrollo Java'),
(27, 6, 'Escribir programas básicos en Java'),
(28, 6, 'Manejar tipos de datos y operadores'),
(29, 6, 'Aplicar estructuras de control'),
(30, 6, 'Trabajar con arreglos y matrices');

-- Curso 7: Python Básico
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(31, 7, 'Comprender la sintaxis de Python'),
(32, 7, 'Utilizar tipos de datos y estructuras nativas'),
(33, 7, 'Trabajar con listas, tuplas y diccionarios'),
(34, 7, 'Crear funciones modulares'),
(35, 7, 'Desarrollar scripts para automatización');

-- Curso 8: Desarrollo Frontend
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(36, 8, 'Estructurar páginas web con HTML5'),
(37, 8, 'Aplicar estilos con CSS3'),
(38, 8, 'Programar interactividad con JavaScript'),
(39, 8, 'Manipular el DOM y eventos'),
(40, 8, 'Crear diseños responsivos');

-- Curso 9: APIs REST
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(41, 9, 'Comprender el concepto de APIs REST'),
(42, 9, 'Utilizar métodos HTTP correctamente'),
(43, 9, 'Manejar JSON y XML'),
(44, 9, 'Implementar autenticación con tokens'),
(45, 9, 'Documentar APIs con Swagger');

-- Curso 10: Git y GitHub
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(46, 10, 'Dominar comandos básicos de Git'),
(47, 10, 'Gestionar ramas y fusiones'),
(48, 10, 'Colaborar en GitHub'),
(49, 10, 'Resolver conflictos de código'),
(50, 10, 'Implementar flujo de trabajo profesional');


-- CATEGORÍA 2: DISEÑO GRÁFICO (idCursos 11 al 20)


-- Curso 11: Diseño Gráfico Básico
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(51, 11, 'Comprender los principios fundamentales del diseño'),
(52, 11, 'Aplicar teoría del color en proyectos'),
(53, 11, 'Seleccionar tipografías adecuadas'),
(54, 11, 'Crear composiciones balanceadas'),
(55, 11, 'Usar herramientas de diseño básicas');

-- Curso 12: Adobe Photoshop
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(56, 12, 'Navegar por la interfaz de Photoshop'),
(57, 12, 'Trabajar con capas y máscaras'),
(58, 12, 'Realizar ajustes de color profesionales'),
(59, 12, 'Aplicar técnicas de retoque fotográfico'),
(60, 12, 'Crear composiciones avanzadas');

-- Curso 13: Adobe Illustrator
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(61, 13, 'Dominar las herramientas de dibujo vectorial'),
(62, 13, 'Crear trazados y curvas complejas'),
(63, 13, 'Aplicar colores y degradados'),
(64, 13, 'Diseñar logotipos profesionales'),
(65, 13, 'Exportar archivos para diferentes formatos');

-- Curso 14: Diseño de Logos
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(66, 14, 'Identificar tipos de logotipos'),
(67, 14, 'Aplicar proceso creativo de diseño'),
(68, 14, 'Crear bocetos y conceptualizar ideas'),
(69, 14, 'Digitalizar propuestas de logo'),
(70, 14, 'Presentar proyectos a clientes');

-- Curso 15: Diseño Publicitario
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(71, 15, 'Comprender los fundamentos de publicidad'),
(72, 15, 'Aplicar psicología del color en anuncios'),
(73, 15, 'Diseñar piezas publicitarias efectivas'),
(74, 15, 'Crear contenido para marketing digital'),
(75, 15, 'Desarrollar campañas publicitarias');

-- Curso 16: Edición de Video
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(76, 16, 'Comprender el flujo de edición de video'),
(77, 16, 'Realizar cortes y montaje básico'),
(78, 16, 'Aplicar transiciones y efectos'),
(79, 16, 'Trabajar con audio y titulación'),
(80, 16, 'Exportar video en diferentes formatos');

-- Curso 17: Diseño UI/UX
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(81, 17, 'Diferenciar entre UI y UX'),
(82, 17, 'Realizar investigación de usuarios'),
(83, 17, 'Crear wireframes y prototipos'),
(84, 17, 'Diseñar interfaces atractivas'),
(85, 17, 'Realizar pruebas de usabilidad');

-- Curso 18: Canva Profesional
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(86, 18, 'Navegar por la plataforma Canva'),
(87, 18, 'Utilizar herramientas básicas de diseño'),
(88, 18, 'Aplicar plantillas profesionales'),
(89, 18, 'Crear contenido para redes sociales'),
(90, 18, 'Desarrollar branding con Canva');

-- Curso 19: Animación Digital
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(91, 19, 'Comprender principios de animación'),
(92, 19, 'Crear storyboards y planificación'),
(93, 19, 'Desarrollar animaciones 2D'),
(94, 19, 'Aplicar motion graphics'),
(95, 19, 'Exportar animaciones correctamente');

-- Curso 20: Branding Empresarial
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(96, 20, 'Entender el concepto de branding'),
(97, 20, 'Desarrollar identidad corporativa'),
(98, 20, 'Crear manual de marca'),
(99, 20, 'Implementar estrategia de marca'),
(100, 20, 'Posicionar marcas en el mercado');


-- CATEGORÍA 3: REDES


-- Curso 21: Fundamentos de Redes
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(101, 21, 'Comprender conceptos básicos de redes'),
(102, 21, 'Identificar topologías de red'),
(103, 21, 'Explicar el modelo OSI'),
(104, 21, 'Configurar direccionamiento IP'),
(105, 21, 'Realizar subneteo básico');

-- Curso 22: Cisco CCNA Básico
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(106, 22, 'Configurar equipos Cisco básicos'),
(107, 22, 'Implementar enrutamiento estático'),
(108, 22, 'Configurar enrutamiento dinámico'),
(109, 22, 'Crear y gestionar VLANs'),
(110, 22, 'Resolver problemas de conectividad');

-- Curso 23: Ciberseguridad
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(111, 23, 'Identificar amenazas de seguridad'),
(112, 23, 'Aplicar principios de criptografía'),
(113, 23, 'Configurar firewalls básicos'),
(114, 23, 'Implementar antivirus y protección'),
(115, 23, 'Aplicar buenas prácticas de seguridad');

-- Curso 24: Administración de Servidores
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(116, 24, 'Comprender el rol de servidores'),
(117, 24, 'Administrar Windows Server'),
(118, 24, 'Configurar servicios en Linux'),
(119, 24, 'Implementar virtualización'),
(120, 24, 'Gestionar servicios de red');

-- Curso 25: Redes Inalámbricas
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(121, 25, 'Comprender tecnología Wi-Fi'),
(122, 25, 'Configurar routers inalámbricos'),
(123, 25, 'Aplicar estándares 802.11'),
(124, 25, 'Implementar seguridad inalámbrica'),
(125, 25, 'Optimizar cobertura Wi-Fi');

-- Curso 26: Linux para Redes
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(126, 26, 'Dominar comandos básicos de Linux'),
(127, 26, 'Administrar sistemas Linux'),
(128, 26, 'Configurar servicios en Linux'),
(129, 26, 'Implementar firewall iptables'),
(130, 26, 'Gestionar redes en Linux');

-- Curso 27: Cableado Estructurado
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(131, 27, 'Identificar tipos de cables'),
(132, 27, 'Aplicar estándares de cableado'),
(133, 27, 'Usar herramientas de instalación'),
(134, 27, 'Realizar certificación de cableado'),
(135, 27, 'Implementar soluciones de cableado');

-- Curso 28: Firewall y Seguridad
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(136, 28, 'Comprender función de firewalls'),
(137, 28, 'Configurar reglas de firewall'),
(138, 28, 'Implementar políticas de seguridad'),
(139, 28, 'Monitorear logs de seguridad'),
(140, 28, 'Proteger perímetros de red');

-- Curso 29: Virtualización
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(141, 29, 'Comprender virtualización'),
(142, 29, 'Configurar VMware básico'),
(143, 29, 'Usar VirtualBox'),
(144, 29, 'Implementar contenedores Docker'),
(145, 29, 'Optimizar recursos virtualizados');

-- Curso 30: Cloud Computing
INSERT INTO objetivos (idObjetivo, curso_id, descripcion) VALUES
(146, 30, 'Diferenciar modelos de cloud'),
(147, 30, 'Usar AWS básico'),
(148, 30, 'Configurar Azure'),
(149, 30, 'Implementar Google Cloud'),
(150, 30, 'Desplegar aplicaciones en cloud');

------*****ACTUALIZAR PRECIOS



-- CATEGORIA 1: PROGRAMACION (idCat = 1)


UPDATE cursos SET precio = 30.00 WHERE idCursos = 2;   -- Programacion en C#
UPDATE cursos SET precio = 30.00 WHERE idCursos = 10;  -- Git y GitHub
UPDATE cursos SET precio = 80.00 WHERE idCursos = 4;   -- Desarrollo Web ASP.NET
UPDATE cursos SET precio = 80.00 WHERE idCursos = 5;   -- Programacion Orientada a Objetos
UPDATE cursos SET precio = 80.00 WHERE idCursos = 8;   -- Desarrollo Frontend
UPDATE cursos SET precio = 99.99 WHERE idCursos = 9;   -- APIs REST



-- CATEGORIA 2: DISENO GRAFICO (idCat = 2)



UPDATE cursos SET precio = 30.00 WHERE idCursos = 11;  -- Diseno Grafico Basico
UPDATE cursos SET precio = 30.00 WHERE idCursos = 14;  -- Diseno de Logos
UPDATE cursos SET precio = 80.00 WHERE idCursos = 12;  -- Adobe Photoshop
UPDATE cursos SET precio = 80.00 WHERE idCursos = 15;  -- Diseno Publicitario
UPDATE cursos SET precio = 80.00 WHERE idCursos = 18;  -- Canva Profesional
UPDATE cursos SET precio = 99.99 WHERE idCursos = 17;  -- Diseno UI/UX


-- CATEGORIA 3: REDES (idCat = 3)

UPDATE cursos SET precio = 30.00 WHERE idCursos = 21;  -- Fundamentos de Redes
UPDATE cursos SET precio = 30.00 WHERE idCursos = 25;  -- Redes Inalambricas
UPDATE cursos SET precio = 80.00 WHERE idCursos = 22;  -- Cisco CCNA Basico
UPDATE cursos SET precio = 80.00 WHERE idCursos = 26;  -- Linux para Redes
UPDATE cursos SET precio = 80.00 WHERE idCursos = 27;  -- Cableado Estructurado
UPDATE cursos SET precio = 99.99 WHERE idCursos = 23;  -- Ciberseguridad



---***-***-AÑADIDO DE INSCRIPCION -EJEMPLO-  -***--**
SELECT idUsuarios, username, nombre FROM usuarios WHERE username = 'juanmartinez';
GO

DECLARE @MaxId INT;
SELECT @MaxId = ISNULL(MAX(idInscripciones), 0) FROM inscripciones;

-- Insertar inscripciones (ajusta el usuario_id según el ID real de juanmartinez)
INSERT INTO inscripciones (idInscripciones, usuario_id, curso_id, progreso, fecha_inscripcion) VALUES
(@MaxId + 1, 8, 1, 0, GETDATE()),   
(@MaxId + 2, 8, 2, 25.5, GETDATE()), 
(@MaxId + 3, 8, 5, 50, GETDATE()),   
(@MaxId + 4, 8, 11, 10, GETDATE()),  
(@MaxId + 5, 8, 21, 75, GETDATE());  
GO


--**--**--**-***--**----*----**-----*---









--CONSULTAS
select * from modulosCurso
select * from cursos
SELECT 
    c.idCursos,
    c.titulo,
    cat.nombre AS categoria,
    c.precio,
    CASE 
        WHEN c.precio = 30 THEN 'Basico - $30'
        WHEN c.precio = 80 THEN 'Intermedio - $80'
        WHEN c.precio = 99.99 THEN 'Avanzado - $99.99'
        ELSE 'Gratis'
    END AS nivel
FROM cursos c
INNER JOIN categorias cat ON c.categoria_id = cat.idCat
WHERE c.precio > 0
ORDER BY cat.idCat, c.precio;

SELECT 
    t.idTarjeta,
    t.tipo_tarjeta,
    '**** **** **** ' + RIGHT(t.numero_tarjeta, 4) AS tarjeta,
    t.nombre_titular,
    t.fecha_expiracion,
    t.cvv AS CVV 
FROM tarjetas t;

select * from tarjetas

select * from inscripciones

SELECT 
    c.idCursos,
    c.titulo AS curso,
    COUNT(m.idModulo) AS total_modulos
FROM cursos c
LEFT JOIN modulosCurso m ON c.idCursos = m.cursoId
GROUP BY c.idCursos, c.titulo
ORDER BY c.idCursos;

--Por si acaso insertas demas cursos
DELETE FROM modulosCurso;
DBCC CHECKIDENT ('modulosCurso', RESEED, 0);
