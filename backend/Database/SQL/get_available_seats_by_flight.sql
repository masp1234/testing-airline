SELECT s.id, s.identifier
FROM seats s
JOIN airplanes a ON s.airplane_id = a.id
JOIN flights f ON f.flights_airplane_id = a.id
LEFT JOIN tickets t ON t.seat_id = s.id AND t.flight_id = f.id
WHERE f.id = 2 AND t.seat_id IS NULL;
