using Bogus;

using playground.Core;

namespace TaskTitan.Infrastructure.Data; // Or your preferred namespace for test data

/// <summary>
/// Generates fake TaskItem data using the Bogus library (Faker facade style).
/// </summary>
public static class FakeData
{
    // Static list to hold generated tasks
    public static List<TaskItem> TaskItems { get; private set; } = new List<TaskItem>();

    // The shared Faker instance
    private static Faker? f;

    // Keep track of the last used ID for sequential generation
    private static int _taskIdCounter = 0;

    /// <summary>
    /// Initializes and generates a specified number of fake TaskItem objects.
    /// </summary>
    /// <param name="count">The number of TaskItems to generate.</param>
    /// <param name="seed">Optional seed for reproducible fake data.</param>
    public static void Init(int count, int? seed = null)
    {
        // Reset lists and counter for fresh generation
        TaskItems.Clear();
        _taskIdCounter = 0;

        // Instantiate the Faker facade
        f = seed.HasValue ? new Faker(locale: "en") : new Faker(); // "en" locale is default but explicit
        if (seed.HasValue)
        {
            Randomizer.Seed = new Random(seed.Value);
        }

        // Generate the tasks
        GenerateTasks(count);
    }

    /// <summary>
    /// Generates the specified number of TaskItem objects.
    /// </summary>
    /// <param name="count">Number of tasks to generate.</param>
    private static void GenerateTasks(int count)
    {
        if (f == null)
        {
            throw new InvalidOperationException("Faker instance not initialized. Call Init first.");
        }

        for (int i = 0; i < count; i++)
        {
            // --- Generate Primitive Values ---
            var taskId = TaskId.From(++_taskIdCounter); // Generate sequential ID
            var taskUuid = TaskUuid.From(Guid.CreateVersion7()); // Or Guid.CreateVersion7()
            var taskDescription = TaskDescription.From(f.Lorem.Sentence(5, 5));
            var entryDate = EntryDate.From(f.Date.PastOffset(2));
            var status = f.PickRandom<TaskItemStatus>();

            // Project (nullable)
            ProjectName? project = f.Random.Bool(0.8f) ? ProjectName.From(f.Commerce.Department(1)) : null;

            // Tags (List<TagName>)
            var tagCount = f.Random.Number(1, 3);
            var tags = new List<TagName>(tagCount);
            for (int j = 0; j < tagCount; j++)
            {
                string tagValue;
                bool isValid;
                do
                {
                    tagValue = f.Random.Word().Replace(" ", "").Replace(",", "");
                    isValid = !string.IsNullOrWhiteSpace(tagValue) && !tagValue.Any(c => char.IsWhiteSpace(c) || c == ',');
                } while (!isValid || string.IsNullOrEmpty(tagValue));
                tags.Add(TagName.From(tagValue));
            }

            // --- Handle Dependent Dates and Nullables ---
            ModifiedDate? modified = null;
            if (status != TaskItemStatus.Pending && f.Random.Bool(0.9f))
            {
                modified = ModifiedDate.From(f.Date.SoonOffset(30, entryDate.Value));
            }

            EndDate? end = null;
            if (status == TaskItemStatus.Completed || status == TaskItemStatus.Deleted)
            {
                var endDateRef = modified?.Value ?? entryDate.Value;
                end = EndDate.From(f.Date.SoonOffset(10, endDateRef));
            }

            DueDate? due = null;
            if (f.Random.Bool(0.7f))
            {
                var dueRef = (status == TaskItemStatus.Pending) ? entryDate.Value : (end?.Value ?? entryDate.Value);
                due = DueDate.From(f.Date.FutureOffset(1, dueRef));
            }

            WaitDate? wait = null;
            if (status == TaskItemStatus.Pending && f.Random.Bool(0.2f))
            {
                wait = WaitDate.From(f.Date.FutureOffset(1, entryDate.Value));
            }

            ScheduledDate? scheduled = null;
            if (status == TaskItemStatus.Pending && f.Random.Bool(0.3f))
            {
                scheduled = ScheduledDate.From(f.Date.FutureOffset(1, entryDate.Value));
            }

            // --- Create TaskItem Instance ---
            // Ensure the order matches the record constructor definition
            var taskItem = new TaskItem(
                Uuid: taskUuid,
                Id: taskId, // Note: Using generated ID, might need null if DB generates it
                Description: taskDescription,
                Project: project,
                Entry: entryDate,
                Modified: modified,
                End: end,
                Due: due,
                Wait: wait,
                Status: status,
                Scheduled: scheduled
            );

            // Add to the static list
            TaskItems.Add(taskItem);
        }
    }
}