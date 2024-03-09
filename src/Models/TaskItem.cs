using System.ComponentModel.DataAnnotations;

namespace ToDoList.Client.Models;

public enum TaskStatus
{
    Todo,
    InProgress,
    Done,
}   

public class TaskItem
{
    public int Id { get; set; }

    [Required]
    public required string Title { get; set; } = string.Empty;

    public TaskStatus Status { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime DateTime { get; set; }
}