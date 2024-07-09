CREATE TABLE tasks (
    id          TEXT NOT NULL
                     CONSTRAINT PK_tasks PRIMARY KEY,
    description TEXT NOT NULL,
    project     TEXT,
    status      TEXT NOT NULL
                     DEFAULT 'pending',
    entry       TEXT NOT NULL
                     DEFAULT (CURRENT_TIMESTAMP),
    modified    TEXT NOT NULL
                     DEFAULT (CURRENT_TIMESTAMP),
    due         TEXT,
    until       TEXT,
    wait        TEXT,
    start       TEXT,
    end         TEXT,
    Scheduled   TEXT
);

-- Trigger: tasks_on_update
CREATE TRIGGER tasks_on_update
         AFTER UPDATE
            ON tasks
      FOR EACH ROW
BEGIN
    UPDATE tasks
       SET modified = CURRENT_TIMESTAMP
     WHERE id = NEW.id;
END;

-- Trigger: tasks_on_insert
CREATE TRIGGER tasks_on_insert
         AFTER INSERT
            ON tasks
      FOR EACH ROW
BEGIN
    UPDATE tasks
       SET modified = CURRENT_TIMESTAMP,
           entry = CURRENT_TIMESTAMP
     WHERE id = NEW.id;
END;

-- View: tasks_with_rowId
CREATE VIEW tasks_with_rowId AS
  SELECT *,
         row_number() OVER (ORDER BY Entry) RowId
    FROM tasks;
