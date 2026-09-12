-- =========================================================================
-- 01_create_tables.sql
-- Esquema relacional: tablas paramétricas (país, departamento, municipio)
-- y tabla de usuario. Motor: PostgreSQL.
-- =========================================================================

CREATE SCHEMA IF NOT EXISTS registro;
SET search_path TO registro;

-- ------------------------------------------------------------------------
-- Tablas paramétricas
-- ------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS pais (
    id_pais     SERIAL PRIMARY KEY,
    codigo      VARCHAR(5)  NOT NULL UNIQUE,   -- ej: CO, MX, PE
    nombre      VARCHAR(100) NOT NULL,
    activo      BOOLEAN      NOT NULL DEFAULT TRUE,
    CONSTRAINT uq_pais_nombre UNIQUE (nombre)
);

CREATE TABLE IF NOT EXISTS departamento (
    id_departamento SERIAL PRIMARY KEY,
    id_pais         INTEGER      NOT NULL REFERENCES pais (id_pais) ON DELETE RESTRICT,
    codigo          VARCHAR(10)  NOT NULL,
    nombre          VARCHAR(100) NOT NULL,
    activo          BOOLEAN      NOT NULL DEFAULT TRUE,
    CONSTRAINT uq_departamento_pais_codigo UNIQUE (id_pais, codigo),
    CONSTRAINT uq_departamento_pais_nombre UNIQUE (id_pais, nombre)
);

CREATE TABLE IF NOT EXISTS municipio (
    id_municipio    SERIAL PRIMARY KEY,
    id_departamento INTEGER      NOT NULL REFERENCES departamento (id_departamento) ON DELETE RESTRICT,
    codigo          VARCHAR(10)  NOT NULL,
    nombre          VARCHAR(100) NOT NULL,
    activo          BOOLEAN      NOT NULL DEFAULT TRUE,
    CONSTRAINT uq_municipio_departamento_codigo UNIQUE (id_departamento, codigo),
    CONSTRAINT uq_municipio_departamento_nombre UNIQUE (id_departamento, nombre)
);

CREATE INDEX IF NOT EXISTS ix_departamento_id_pais ON departamento (id_pais);
CREATE INDEX IF NOT EXISTS ix_municipio_id_departamento ON municipio (id_departamento);

-- ------------------------------------------------------------------------
-- Tabla de usuario
-- ------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS usuario (
    id_usuario      SERIAL PRIMARY KEY,
    nombre          VARCHAR(150) NOT NULL,
    telefono        VARCHAR(20)  NOT NULL,
    id_pais         INTEGER      NOT NULL REFERENCES pais (id_pais) ON DELETE RESTRICT,
    id_departamento INTEGER      NOT NULL REFERENCES departamento (id_departamento) ON DELETE RESTRICT,
    id_municipio    INTEGER      NOT NULL REFERENCES municipio (id_municipio) ON DELETE RESTRICT,
    direccion       VARCHAR(250) NOT NULL,
    fecha_registro  TIMESTAMPTZ  NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS ix_usuario_id_municipio ON usuario (id_municipio);
