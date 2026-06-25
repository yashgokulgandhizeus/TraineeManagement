using System.Configuration;

namespace TraineeManagement.Api.Services;

public static class CacheKeys
{
    public static string Trainee(int id) => $"trainee:{id}";

    public static string TraineeList(int page,int size,string? search,string status)=>$"trainee:list:{page}:{size}:{search ??"all"}:{status ??"all"}";

    public static string TaskAssignment(int id)=> $"task-assignment:{id}";
    
    public static string TaskAssignmentList()=>$"task-assignment:list";

    public static string Submission(int id)=> $"submission:{id}";

    public static string SubmissionList()=>$"submission:list";


}