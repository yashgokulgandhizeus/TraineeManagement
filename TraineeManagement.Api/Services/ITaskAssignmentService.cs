namespace TraineeManagement.Api.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using TraineeManagement.Api.Dtos;

public interface ITaskAssignmentService
{
   Task<TaskAssignmentResponse> TaskAssignment(TaskAssignmentRequest request);

   Task<List<TaskAssignmentResponse>> GetAll();

   Task<TaskAssignmentResponse> GetById(int id);

   Task<TaskAssignmentResponse> UpdateStatus(int id, AssignmentStatus status);
}
