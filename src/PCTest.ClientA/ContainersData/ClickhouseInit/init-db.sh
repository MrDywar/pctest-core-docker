#!/bin/bash
set -e

clickhouse client -n <<-EOSQL
    CREATE DATABASE IF NOT EXISTS test;

    CREATE TABLE IF NOT EXISTS test.User
    (
      ID Int64,
      Name String,
      Age Int32 
    ) ENGINE = MergeTree()
    PRIMARY KEY ID
    ORDER BY ID;

EOSQL