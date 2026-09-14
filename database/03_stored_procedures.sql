SET search_path TO registro;

CREATE OR REPLACE FUNCTION sp_pais_listar()
RETURNS TABLE (
    id_pais INTEGER,
    codigo  VARCHAR,
    nombre  VARCHAR
)
LANGUAGE sql
STABLE
SET search_path = registro
AS $$
    SELECT id_pais, codigo, nombre
    FROM pais
    WHERE activo = TRUE
    ORDER BY nombre;
$$;

CREATE OR REPLACE FUNCTION sp_departamento_listar_por_pais(p_id_pais INTEGER)
RETURNS TABLE (
    id_departamento INTEGER,
    id_pais         INTEGER,
    codigo          VARCHAR,
    nombre          VARCHAR
)
LANGUAGE sql
STABLE
SET search_path = registro
AS $$
    SELECT id_departamento, id_pais, codigo, nombre
    FROM departamento
    WHERE id_pais = p_id_pais
      AND activo = TRUE
    ORDER BY nombre;
$$;

CREATE OR REPLACE FUNCTION sp_municipio_listar_por_departamento(p_id_departamento INTEGER)
RETURNS TABLE (
    id_municipio    INTEGER,
    id_departamento INTEGER,
    codigo          VARCHAR,
    nombre          VARCHAR
)
LANGUAGE sql
STABLE
SET search_path = registro
AS $$
    SELECT id_municipio, id_departamento, codigo, nombre
    FROM municipio
    WHERE id_departamento = p_id_departamento
      AND activo = TRUE
    ORDER BY nombre;
$$;

CREATE OR REPLACE FUNCTION sp_ubicacion_validar(
    p_id_pais         INTEGER,
    p_id_departamento INTEGER,
    p_id_municipio    INTEGER
)
RETURNS TABLE (
    pais_existe                    BOOLEAN,
    departamento_existe             BOOLEAN,
    departamento_pertenece_a_pais   BOOLEAN,
    municipio_existe                BOOLEAN,
    municipio_pertenece_a_departamento BOOLEAN
)
LANGUAGE sql
STABLE
SET search_path = registro
AS $$
    SELECT
        EXISTS (SELECT 1 FROM pais WHERE id_pais = p_id_pais AND activo = TRUE),
        EXISTS (SELECT 1 FROM departamento WHERE id_departamento = p_id_departamento AND activo = TRUE),
        EXISTS (
            SELECT 1 FROM departamento
            WHERE id_departamento = p_id_departamento
              AND id_pais = p_id_pais
        ),
        EXISTS (SELECT 1 FROM municipio WHERE id_municipio = p_id_municipio AND activo = TRUE),
        EXISTS (
            SELECT 1 FROM municipio
            WHERE id_municipio = p_id_municipio
              AND id_departamento = p_id_departamento
        );
$$;

CREATE OR REPLACE FUNCTION sp_usuario_obtener(p_numero_documento VARCHAR)
RETURNS TABLE (
    numero_documento    VARCHAR,
    nombre              VARCHAR,
    telefono            VARCHAR,
    id_pais             INTEGER,
    pais_nombre         VARCHAR,
    id_departamento     INTEGER,
    departamento_nombre VARCHAR,
    id_municipio        INTEGER,
    municipio_nombre    VARCHAR,
    direccion           VARCHAR,
    fecha_registro      TIMESTAMPTZ
)
LANGUAGE sql
STABLE
SET search_path = registro
AS $$
    SELECT u.numero_documento, u.nombre, u.telefono,
           u.id_pais, p.nombre,
           u.id_departamento, d.nombre,
           u.id_municipio, m.nombre,
           u.direccion, u.fecha_registro
    FROM usuario u
    JOIN pais p ON p.id_pais = u.id_pais
    JOIN departamento d ON d.id_departamento = u.id_departamento
    JOIN municipio m ON m.id_municipio = u.id_municipio
    WHERE u.numero_documento = p_numero_documento;
$$;

CREATE OR REPLACE FUNCTION sp_usuario_registrar(
    p_numero_documento VARCHAR,
    p_nombre          VARCHAR,
    p_telefono        VARCHAR,
    p_id_pais         INTEGER,
    p_id_departamento INTEGER,
    p_id_municipio    INTEGER,
    p_direccion       VARCHAR
)
RETURNS TABLE (
    numero_documento    VARCHAR,
    nombre              VARCHAR,
    telefono            VARCHAR,
    id_pais             INTEGER,
    pais_nombre         VARCHAR,
    id_departamento     INTEGER,
    departamento_nombre VARCHAR,
    id_municipio        INTEGER,
    municipio_nombre    VARCHAR,
    direccion           VARCHAR,
    fecha_registro      TIMESTAMPTZ
)
LANGUAGE plpgsql
SET search_path = registro
AS $$
BEGIN
    INSERT INTO usuario (numero_documento, nombre, telefono, id_pais, id_departamento, id_municipio, direccion)
    VALUES (p_numero_documento, p_nombre, p_telefono, p_id_pais, p_id_departamento, p_id_municipio, p_direccion);

    RETURN QUERY
    SELECT * FROM sp_usuario_obtener(p_numero_documento);
END;
$$;
