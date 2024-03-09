using Blazored.LocalStorage;
using ToDoList.Client.Models;
using TaskStatus = ToDoList.Client.Models.TaskStatus;

namespace ToDoList.Client.Services;

public class TaskService
{
    private ISyncLocalStorageService _localStorage;

    public IEnumerable<TaskItem> AllTasks => _tasks;
    
    private List<TaskItem> _tasks;


    public TaskService(ISyncLocalStorageService localStorage)
    {
        _localStorage = localStorage;
        _tasks = localStorage.GetItem<List<TaskItem>>("todos") ?? new List<TaskItem>();
    }

    public IEnumerable<TaskItem> GetSubTask(int start = 0, int end = 0)
    {
        // Calculate the start index of the page
        var startIndex = (start - 1) * end;

        // Select the tasks for the current page using Skip and Take
        var tasksForPage = _tasks.Skip(startIndex).Take(end);

        // Convert the IEnumerable<TaskItem> to List<TaskItem> if needed
        return tasksForPage.ToList();
    }
    
    public void AddTask(TaskItem task)
    {
        // Assign a unique ID to the task
        task.Id = _tasks.Any() ? _tasks.Max(t => t.Id + 1) : 1;
        _tasks.Add(task);
        task.DateTime = DateTime.Now;
        Save();
    }

    public void UpdateTask(TaskItem task)
    {
        var existingTask = _tasks.FirstOrDefault(t => t.Id == task.Id);
        if (existingTask == null) return;

        existingTask.Title = task.Title;
        existingTask.DateTime = DateTime.Now;

        // Update other properties as needed
        Save();
    }

    public void ClearTask(TaskItem task)
    {
        var existingTask = _tasks.FirstOrDefault(t => t.Id == task.Id);
        if (existingTask == null) return;

        _tasks.Remove(existingTask);
        RecalculateTaskIds();
        Save();
    }

    public void ShuffleTaskStatus(TaskItem task)
    {
        // Shuffle the task status based on your logic
        task.Status = task.Status switch
        {
            TaskStatus.Todo => TaskStatus.InProgress,
            TaskStatus.InProgress => TaskStatus.Done,
            TaskStatus.Done => TaskStatus.Todo,
            _ => task.Status
        };
        MarkIsCompleted(task);
        Save();
    }


    public void ClearAllTasks()
    {
        _tasks.Clear();
        _localStorage.RemoveItem("todos");
    }

    public void MarkAllDone()
    {
        foreach (var task in _tasks.Where(task => !task.IsCompleted))
        {
            task.Status = TaskStatus.Done;
            MarkIsCompleted(task);
        }

        Save();
    }

    private static void MarkIsCompleted(TaskItem task)
    {
        task.IsCompleted = task.Status == TaskStatus.Done;
    }

    private void RecalculateTaskIds()
    {
        for (var i = 0; i < _tasks.Count; i++)
        {
            _tasks[i].Id = i + 1;
        }
    }

    private void Save()
    {
        _localStorage.SetItem("todos", _tasks);
    }
}