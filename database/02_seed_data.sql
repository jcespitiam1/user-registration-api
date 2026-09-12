-- =========================================================================
-- 02_seed_data.sql
-- Datos base para las tablas paramétricas: país, departamento, municipio.
-- Incluye Colombia (con sus departamentos y algunos municipios) y dos
-- países adicionales (México y Perú) para poder validar la coherencia
-- referencial entre país / departamento / municipio.
-- =========================================================================

SET search_path TO registro;

-- ------------------------------------------------------------------------
-- Países
-- ------------------------------------------------------------------------
INSERT INTO pais (codigo, nombre) VALUES
    ('CO', 'Colombia'),
    ('MX', 'México'),
    ('PE', 'Perú')
ON CONFLICT (codigo) DO NOTHING;

-- ------------------------------------------------------------------------
-- Departamentos / Estados
-- ------------------------------------------------------------------------
INSERT INTO departamento (id_pais, codigo, nombre)
SELECT p.id_pais, v.codigo, v.nombre
FROM pais p
JOIN (VALUES
    -- Colombia
    ('CO', 'AMA', 'Amazonas'),
    ('CO', 'ANT', 'Antioquia'),
    ('CO', 'ARA', 'Arauca'),
    ('CO', 'ATL', 'Atlántico'),
    ('CO', 'BOG', 'Bogotá D.C.'),
    ('CO', 'BOL', 'Bolívar'),
    ('CO', 'BOY', 'Boyacá'),
    ('CO', 'CAL', 'Caldas'),
    ('CO', 'CAQ', 'Caquetá'),
    ('CO', 'CAS', 'Casanare'),
    ('CO', 'CAU', 'Cauca'),
    ('CO', 'CES', 'Cesar'),
    ('CO', 'CHO', 'Chocó'),
    ('CO', 'COR', 'Córdoba'),
    ('CO', 'CUN', 'Cundinamarca'),
    ('CO', 'GUA', 'Guainía'),
    ('CO', 'GUV', 'Guaviare'),
    ('CO', 'HUI', 'Huila'),
    ('CO', 'LAG', 'La Guajira'),
    ('CO', 'MAG', 'Magdalena'),
    ('CO', 'MET', 'Meta'),
    ('CO', 'NAR', 'Nariño'),
    ('CO', 'NSA', 'Norte de Santander'),
    ('CO', 'PUT', 'Putumayo'),
    ('CO', 'QUI', 'Quindío'),
    ('CO', 'RIS', 'Risaralda'),
    ('CO', 'SAP', 'San Andrés y Providencia'),
    ('CO', 'SAN', 'Santander'),
    ('CO', 'SUC', 'Sucre'),
    ('CO', 'TOL', 'Tolima'),
    ('CO', 'VAC', 'Valle del Cauca'),
    ('CO', 'VAU', 'Vaupés'),
    ('CO', 'VID', 'Vichada'),
    -- México
    ('MX', 'CDMX', 'Ciudad de México'),
    ('MX', 'JAL', 'Jalisco'),
    ('MX', 'NLE', 'Nuevo León'),
    ('MX', 'MEX', 'Estado de México'),
    ('MX', 'YUC', 'Yucatán'),
    -- Perú
    ('PE', 'LIM', 'Lima'),
    ('PE', 'ARE', 'Arequipa'),
    ('PE', 'CUS', 'Cusco'),
    ('PE', 'LAL', 'La Libertad'),
    ('PE', 'PIU', 'Piura')
) AS v(pais_codigo, codigo, nombre) ON v.pais_codigo = p.codigo
ON CONFLICT (id_pais, codigo) DO NOTHING;

-- ------------------------------------------------------------------------
-- Municipios
-- ------------------------------------------------------------------------
INSERT INTO municipio (id_departamento, codigo, nombre)
SELECT d.id_departamento, v.mun_codigo, v.nombre
FROM departamento d
JOIN pais p ON p.id_pais = d.id_pais
JOIN (VALUES
    -- Colombia
    ('CO', 'AMA', 'LET', 'Leticia'),
    ('CO', 'ANT', 'MED', 'Medellín'),
    ('CO', 'ANT', 'ENV', 'Envigado'),
    ('CO', 'ARA', 'ARC', 'Arauca'),
    ('CO', 'ARA', 'SAR', 'Saravena'),
    ('CO', 'ATL', 'BAQ', 'Barranquilla'),
    ('CO', 'ATL', 'SOL', 'Soledad'),
    ('CO', 'BOG', 'BOG', 'Bogotá D.C.'),
    ('CO', 'BOL', 'CTG', 'Cartagena'),
    ('CO', 'BOL', 'MAN', 'Magangué'),
    ('CO', 'BOY', 'TUN', 'Tunja'),
    ('CO', 'BOY', 'DUI', 'Duitama'),
    ('CO', 'CAL', 'MZL', 'Manizales'),
    ('CO', 'CAL', 'DOR', 'La Dorada'),
    ('CO', 'CAQ', 'FLO', 'Florencia'),
    ('CO', 'CAS', 'YOP', 'Yopal'),
    ('CO', 'CAU', 'POP', 'Popayán'),
    ('CO', 'CES', 'VAL', 'Valledupar'),
    ('CO', 'CHO', 'QUI', 'Quibdó'),
    ('CO', 'COR', 'MON', 'Montería'),
    ('CO', 'CUN', 'ZIP', 'Zipaquirá'),
    ('CO', 'CUN', 'SOA', 'Soacha'),
    ('CO', 'CUN', 'FUS', 'Fusagasugá'),
    ('CO', 'GUA', 'INI', 'Inírida'),
    ('CO', 'GUV', 'SJG', 'San José del Guaviare'),
    ('CO', 'HUI', 'NEI', 'Neiva'),
    ('CO', 'LAG', 'RIO', 'Riohacha'),
    ('CO', 'MAG', 'SMR', 'Santa Marta'),
    ('CO', 'MET', 'VVC', 'Villavicencio'),
    ('CO', 'NAR', 'PAS', 'Pasto'),
    ('CO', 'NSA', 'CUC', 'Cúcuta'),
    ('CO', 'PUT', 'MOC', 'Mocoa'),
    ('CO', 'QUI', 'ARM', 'Armenia'),
    ('CO', 'RIS', 'PER', 'Pereira'),
    ('CO', 'SAP', 'SAI', 'San Andrés'),
    ('CO', 'SAN', 'BGA', 'Bucaramanga'),
    ('CO', 'SAN', 'FLB', 'Floridablanca'),
    ('CO', 'SUC', 'SIN', 'Sincelejo'),
    ('CO', 'TOL', 'IBA', 'Ibagué'),
    ('CO', 'VAC', 'CAL', 'Cali'),
    ('CO', 'VAC', 'PAL', 'Palmira'),
    ('CO', 'VAC', 'BUE', 'Buenaventura'),
    ('CO', 'VAU', 'MIT', 'Mitú'),
    ('CO', 'VID', 'PCA', 'Puerto Carreño'),
    -- México
    ('MX', 'CDMX', 'CUA', 'Cuauhtémoc'),
    ('MX', 'CDMX', 'COY', 'Coyoacán'),
    ('MX', 'JAL', 'GDL', 'Guadalajara'),
    ('MX', 'JAL', 'ZAP', 'Zapopan'),
    ('MX', 'NLE', 'MTY', 'Monterrey'),
    ('MX', 'NLE', 'SPG', 'San Pedro Garza García'),
    ('MX', 'MEX', 'TOL', 'Toluca'),
    ('MX', 'MEX', 'ECA', 'Ecatepec'),
    ('MX', 'YUC', 'MER', 'Mérida'),
    ('MX', 'YUC', 'VAL', 'Valladolid'),
    -- Perú
    ('PE', 'LIM', 'LIM', 'Lima'),
    ('PE', 'LIM', 'MIR', 'Miraflores'),
    ('PE', 'ARE', 'ARE', 'Arequipa'),
    ('PE', 'ARE', 'CAY', 'Cayma'),
    ('PE', 'CUS', 'CUS', 'Cusco'),
    ('PE', 'CUS', 'URU', 'Urubamba'),
    ('PE', 'LAL', 'TRU', 'Trujillo'),
    ('PE', 'LAL', 'PAC', 'Pacasmayo'),
    ('PE', 'PIU', 'PIU', 'Piura'),
    ('PE', 'PIU', 'SUL', 'Sullana')
) AS v(pais_codigo, depto_codigo, mun_codigo, nombre)
  ON v.pais_codigo = p.codigo AND v.depto_codigo = d.codigo
ON CONFLICT (id_departamento, codigo) DO NOTHING;
