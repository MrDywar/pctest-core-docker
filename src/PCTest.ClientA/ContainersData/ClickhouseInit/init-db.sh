#!/bin/bash
set -e

clickhouse client -n <<-EOSQL
    CREATE DATABASE test;
    CREATE TABLE test.User (ID Int64, Name String, Age Int32 ) ENGINE = MergeTree() PRIMARY KEY ID ORDER BY ID;
    INSERT INTO test.User VALUES (1, 'Dywar', 33);
EOSQL