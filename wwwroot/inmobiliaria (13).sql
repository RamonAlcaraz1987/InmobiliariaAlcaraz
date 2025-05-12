-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 12-05-2025 a las 15:09:44
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliaria`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contratos`
--

CREATE TABLE `contratos` (
  `IdContrato` int(11) NOT NULL,
  `IdInquilino` int(11) NOT NULL,
  `IdInmueble` int(11) NOT NULL,
  `MontoMensual` decimal(10,2) DEFAULT NULL,
  `FechaInicio` date NOT NULL,
  `FechaFin` date NOT NULL,
  `IdUsuarioCreacion` int(11) NOT NULL,
  `IdUsuarioFinalizacion` int(11) DEFAULT NULL,
  `FechaCreacion` datetime NOT NULL DEFAULT current_timestamp(),
  `FechaFinAnticipado` date DEFAULT NULL,
  `PagosEsperados` int(11) NOT NULL,
  `Estado` int(11) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contratos`
--

INSERT INTO `contratos` (`IdContrato`, `IdInquilino`, `IdInmueble`, `MontoMensual`, `FechaInicio`, `FechaFin`, `IdUsuarioCreacion`, `IdUsuarioFinalizacion`, `FechaCreacion`, `FechaFinAnticipado`, `PagosEsperados`, `Estado`) VALUES
(29, 3, 7, 60000.00, '2025-05-13', '2025-08-13', 4, NULL, '2025-05-12 10:08:41', NULL, 4, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `imagenes`
--

CREATE TABLE `imagenes` (
  `IdImagen` int(11) NOT NULL,
  `IdInmueble` int(11) NOT NULL,
  `PortadaUrl` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmuebles`
--

CREATE TABLE `inmuebles` (
  `IdInmueble` int(20) NOT NULL,
  `Direccion` varchar(200) NOT NULL,
  `IdTipo` int(20) NOT NULL,
  `IdUso` int(11) NOT NULL,
  `IdPropietario` int(20) DEFAULT NULL,
  `Ambientes` int(20) DEFAULT NULL,
  `Latitud` decimal(10,2) DEFAULT NULL,
  `Longitud` decimal(10,2) DEFAULT NULL,
  `Superficie` int(11) DEFAULT NULL,
  `Disponible` int(11) NOT NULL,
  `Precio` decimal(10,2) DEFAULT NULL,
  `PortadaUrl` varchar(200) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmuebles`
--

INSERT INTO `inmuebles` (`IdInmueble`, `Direccion`, `IdTipo`, `IdUso`, `IdPropietario`, `Ambientes`, `Latitud`, `Longitud`, `Superficie`, `Disponible`, `Precio`, `PortadaUrl`) VALUES
(7, 'colon 740', 4, 2, 14, 1, 14587.00, 14.00, 50, 1, 60000.00, NULL),
(8, 'barrio parque centenario 125', 3, 1, 13, 3, 145754.00, 100245.00, 100, 1, 99000.00, NULL),
(9, 'gral paz 497', 2, 1, 15, 4, 1122454.00, 455.00, 150, 1, 90000.00, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilinos`
--

CREATE TABLE `inquilinos` (
  `IdInquilino` int(11) NOT NULL,
  `Nombre` varchar(50) DEFAULT NULL,
  `Apellido` varchar(50) DEFAULT NULL,
  `Dni` varchar(50) DEFAULT NULL,
  `Telefono` varchar(20) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `Direccion` varchar(100) DEFAULT NULL,
  `FechaNacimiento` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilinos`
--

INSERT INTO `inquilinos` (`IdInquilino`, `Nombre`, `Apellido`, `Dni`, `Telefono`, `Email`, `Direccion`, `FechaNacimiento`) VALUES
(3, 'Emanuel', 'Alcaraz', '33442114', '26644658668', 'ramon@gmail.com', 'barrio 130 viviendas manzana 421 c 1', '1987-06-26'),
(4, 'Pablo', 'Gatica', '31577922', '2665761441', 'pablo@gmail.com', 'barrio amep mza 4 c 4', '1991-01-30'),
(5, 'Claudio', 'Olivares', '37544859', '2664774855', 'claudio@gmail.com', 'barrio 123 viv mza 411 c 5', '1987-09-09');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pagos`
--

CREATE TABLE `pagos` (
  `IdPago` int(11) NOT NULL,
  `IdContrato` int(11) NOT NULL,
  `Monto` decimal(10,2) NOT NULL,
  `FechaPago` datetime DEFAULT NULL,
  `NumeroPago` int(11) NOT NULL,
  `Detalle` varchar(255) DEFAULT NULL,
  `Anulado` tinyint(4) NOT NULL DEFAULT 0,
  `IdUsuarioCreacion` int(11) NOT NULL,
  `IdUsuarioAnulacion` int(11) DEFAULT NULL,
  `FechaCreacion` datetime NOT NULL DEFAULT current_timestamp(),
  `fechaAnulacion` datetime DEFAULT NULL,
  `EsMulta` int(11) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietarios`
--

CREATE TABLE `propietarios` (
  `IdPropietario` int(11) NOT NULL,
  `Dni` varchar(50) DEFAULT NULL,
  `Nombre` varchar(50) DEFAULT NULL,
  `Apellido` varchar(200) DEFAULT NULL,
  `Telefono` varchar(100) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `Direccion` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietarios`
--

INSERT INTO `propietarios` (`IdPropietario`, `Dni`, `Nombre`, `Apellido`, `Telefono`, `Email`, `Direccion`) VALUES
(13, '34528558', 'Sebastian', 'Becerra', '2664808880', 'sebastian@gmail.com', 'sarmiento 470'),
(14, '26588777', 'Fabian', 'Garro', '2664758887', 'fabian@gmail.com', 'los lapachos 2 c 58'),
(15, '25478447', 'Victor', 'Garcia', '2665736850', 'victor@gmail.com', 'la punta modulo 3 c 48');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipos`
--

CREATE TABLE `tipos` (
  `IdTipo` int(11) NOT NULL,
  `Descripcion` varchar(200) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tipos`
--

INSERT INTO `tipos` (`IdTipo`, `Descripcion`) VALUES
(1, 'Monoambiente'),
(2, 'Casa'),
(3, 'Departamento'),
(4, 'Local');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usos`
--

CREATE TABLE `usos` (
  `IdUso` int(11) NOT NULL,
  `Descripcion` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usos`
--

INSERT INTO `usos` (`IdUso`, `Descripcion`) VALUES
(1, 'Residencial'),
(2, 'Comercial');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `IdUsuario` int(11) NOT NULL,
  `Nombre` varchar(200) NOT NULL,
  `Apellido` varchar(200) NOT NULL,
  `Email` varchar(200) NOT NULL,
  `Clave` varchar(200) NOT NULL,
  `Avatar` varchar(255) DEFAULT NULL,
  `Rol` tinyint(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`IdUsuario`, `Nombre`, `Apellido`, `Email`, `Clave`, `Avatar`, `Rol`) VALUES
(3, 'Ramon', 'Alcaraz', 'emanuel@gmail.com', 'RzzYOC7izzsNxnlWb47BQ9JZM8UnEfjKU9x/StUV/Rg=', NULL, 2),
(4, 'admin', 'admin', 'admin@admin.com', '+b8lBAGauq80OdALZF/iHcvhsQkBuqqLZ8jsFv6WGgo=', '/Uploads/avatar_4.png', 1);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`IdContrato`),
  ADD KEY `IdInmueble` (`IdInmueble`),
  ADD KEY `IdInquilino` (`IdInquilino`),
  ADD KEY `IdUsuarioCreacion` (`IdUsuarioCreacion`),
  ADD KEY `IdUsuarioFinalizacion` (`IdUsuarioFinalizacion`);

--
-- Indices de la tabla `imagenes`
--
ALTER TABLE `imagenes`
  ADD PRIMARY KEY (`IdImagen`),
  ADD KEY `IdInmueble` (`IdInmueble`);

--
-- Indices de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`IdInmueble`),
  ADD KEY `IdPropietario` (`IdPropietario`),
  ADD KEY `IdTipo` (`IdTipo`),
  ADD KEY `IdUso` (`IdUso`);

--
-- Indices de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`IdInquilino`);

--
-- Indices de la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`IdPago`),
  ADD KEY `IdContrato` (`IdContrato`),
  ADD KEY `IdUsuarioCreacion` (`IdUsuarioCreacion`),
  ADD KEY `IdUsuarioAnulacion` (`IdUsuarioAnulacion`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`IdPropietario`);

--
-- Indices de la tabla `tipos`
--
ALTER TABLE `tipos`
  ADD PRIMARY KEY (`IdTipo`);

--
-- Indices de la tabla `usos`
--
ALTER TABLE `usos`
  ADD PRIMARY KEY (`IdUso`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`IdUsuario`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contratos`
--
ALTER TABLE `contratos`
  MODIFY `IdContrato` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=30;

--
-- AUTO_INCREMENT de la tabla `imagenes`
--
ALTER TABLE `imagenes`
  MODIFY `IdImagen` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `IdInmueble` int(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `IdInquilino` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `IdPago` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `IdPropietario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT de la tabla `tipos`
--
ALTER TABLE `tipos`
  MODIFY `IdTipo` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `usos`
--
ALTER TABLE `usos`
  MODIFY `IdUso` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `IdUsuario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `contratos_ibfk_1` FOREIGN KEY (`IdInmueble`) REFERENCES `inmuebles` (`IdInmueble`),
  ADD CONSTRAINT `contratos_ibfk_2` FOREIGN KEY (`IdInquilino`) REFERENCES `inquilinos` (`IdInquilino`),
  ADD CONSTRAINT `contratos_ibfk_3` FOREIGN KEY (`IdUsuarioCreacion`) REFERENCES `usuarios` (`IdUsuario`),
  ADD CONSTRAINT `contratos_ibfk_4` FOREIGN KEY (`IdUsuarioFinalizacion`) REFERENCES `usuarios` (`IdUsuario`);

--
-- Filtros para la tabla `imagenes`
--
ALTER TABLE `imagenes`
  ADD CONSTRAINT `imagenes_ibfk_1` FOREIGN KEY (`IdInmueble`) REFERENCES `inmuebles` (`IdInmueble`);

--
-- Filtros para la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `inmuebles_ibfk_1` FOREIGN KEY (`IdPropietario`) REFERENCES `propietarios` (`IdPropietario`),
  ADD CONSTRAINT `inmuebles_ibfk_2` FOREIGN KEY (`IdTipo`) REFERENCES `tipos` (`IdTipo`),
  ADD CONSTRAINT `inmuebles_ibfk_3` FOREIGN KEY (`IdUso`) REFERENCES `usos` (`IdUso`);

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `pagos_ibfk_1` FOREIGN KEY (`IdContrato`) REFERENCES `contratos` (`IdContrato`),
  ADD CONSTRAINT `pagos_ibfk_2` FOREIGN KEY (`IdUsuarioCreacion`) REFERENCES `usuarios` (`IdUsuario`),
  ADD CONSTRAINT `pagos_ibfk_3` FOREIGN KEY (`IdUsuarioAnulacion`) REFERENCES `usuarios` (`IdUsuario`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
