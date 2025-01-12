#!/bin/bash

# Indstil miljøvariabler

OUTPUT_DIR="../backup/$(date +%Y-%m-%d)"

# Opret mappe til backup, hvis den ikke findes
mkdir -p $OUTPUT_DIR

# Kør mongodump for at tage backup
mongodump --uri="mongodb+srv://airline:eASxfB2IYqazAy4L@airline-backend.s7zn4.mongodb.net/?retryWrites=true&w=majority&appName=airline-backend" --out=$OUTPUT_DIR

# Kontroller, om dump lykkedes
if [ $? -eq 0 ]; then
  echo "Backup færdig! Filer gemt i $OUTPUT_DIR"
else
  echo "Fejl under backup"
  exit 1
fi