DROP VIew pending_tasks;
CREATE VIEW pending_tasks as
SELECT
    *,
    row_number() OVER ( ORDER BY Created) RowId
FROM tasks
WHERE
    State = "Pending";
	SELECT * FROM pending_tasks