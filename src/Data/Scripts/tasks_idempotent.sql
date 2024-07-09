-- View: tasks_with_rowId

DROP VIEW IF EXISTS tasks_with_rowId;

CREATE VIEW tasks_with_rowId AS
  SELECT *,
         row_number() OVER (ORDER BY Entry) RowId
    FROM tasks;
