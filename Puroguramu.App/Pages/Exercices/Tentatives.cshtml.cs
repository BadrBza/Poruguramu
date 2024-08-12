using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Puroguramu.Domains.modelsDomains;
using Puroguramu.Domains.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Puroguramu.Domains;

namespace Puroguramu.App.Pages
{
    public class TentativesModel : PageModel
    {
        private readonly IExercisesRepository _exercisesRepository;
        private readonly IStudentRepository _studentRepository;

        public string ExerciseTitle { get; set; }
        public string ExerciseStatut { get; set; }
        public IEnumerable<TentativeDto> Tentatives { get; set; }
        public Guid ExerciseId { get; set; }

        public TentativesModel(IExercisesRepository exercisesRepository, IStudentRepository studentRepository)
        {
            _exercisesRepository = exercisesRepository;
            _studentRepository = studentRepository;
        }

        public async Task OnGetAsync(Guid exerciseId)
        {
            ExerciseId = exerciseId;
            var student = await _studentRepository.GetStudentProfileAsync(User);
            var exercise = _exercisesRepository.GetExercise(exerciseId);

            if (exercise != null && student != null)
            {
                ExerciseTitle = exercise.Title;
                ExerciseStatut = exercise.Difficulty.ToString();
                Tentatives = await _studentRepository.GetStudentTentativesAsync(exerciseId, student.Id);
            }
        }
    }
}
