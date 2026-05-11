-- connectionString =
    "Data Source=\"c:\Users\local_user\AppData\Local\GMap.NET\TileDBv5\en\Data.gmdb)\";Page Size=32768;Pooling=True";

--CreateTileDb	
CREATE TABLE IF NOT EXISTS Tiles (id INTEGER NOT NULL PRIMARY KEY, X INTEGER NOT NULL, Y INTEGER NOT NULL, Zoom INTEGER NOT NULL, Type UNSIGNED INTEGER  NOT NULL, CacheTime DATETIME);
CREATE INDEX IF NOT EXISTS IndexOfTiles ON Tiles (X, Y, Zoom, Type);

CREATE TABLE IF NOT EXISTS TilesData (id INTEGER NOT NULL PRIMARY KEY CONSTRAINT fk_Tiles_id REFERENCES Tiles(id) ON DELETE CASCADE, Tile BLOB NULL);

-- Foreign Key Preventing insert
CREATE TRIGGER fki_TilesData_id_Tiles_id
BEFORE INSERT ON [TilesData]
FOR EACH ROW BEGIN
  SELECT RAISE(ROLLBACK, 'insert on table "TilesData" violates foreign key constraint "fki_TilesData_id_Tiles_id"')
  WHERE (SELECT id FROM Tiles WHERE id = NEW.id) IS NULL;
END;

-- Foreign key preventing update
CREATE TRIGGER fku_TilesData_id_Tiles_id
BEFORE UPDATE ON [TilesData] 
FOR EACH ROW BEGIN
    SELECT RAISE(ROLLBACK, 'update on table "TilesData" violates foreign key constraint "fku_TilesData_id_Tiles_id"')
      WHERE (SELECT id FROM Tiles WHERE id = NEW.id) IS NULL;
END;

-- Cascading Delete
CREATE TRIGGER fkdc_TilesData_id_Tiles_id
BEFORE DELETE ON Tiles
FOR EACH ROW BEGIN 
    DELETE FROM TilesData WHERE TilesData.id = OLD.id;
END;	


--singleSqlSelect =
    SELECT Tile FROM main.TilesData WHERE id = (SELECT id FROM main.Tiles WHERE X={0} AND Y={1} AND Zoom={2} AND Type={3});

--singleSqlInsert = 
  INSERT INTO main.Tiles(X, Y, Zoom, Type, CacheTime) VALUES(@p1, @p2, @p3, @p4, @p5);

--singleSqlInsertLast = 
  INSERT INTO main.TilesData(id, Tile) VALUES((SELECT last_insert_rowid()), @p1);
  
-- attachSqlQuery  
      ATTACH '{0}' as db{1}; -- ", c, i);
-- _detachSqlQuery 
    DETACH DATABASE db{0}; -- ", i);