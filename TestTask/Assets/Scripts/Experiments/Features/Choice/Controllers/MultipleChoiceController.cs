using System.Linq;
using Features.Choice.Components;
using Features.Choice.Models;

namespace Features.Choice.Controllers
{
    public class MultipleChoiceController : ChoiceController
    {
        protected override void OnObjectClick(ChoiceComponent obj)
        {
            ChoiceModel chosenObjectModel = 
                objectsToChoose.FirstOrDefault(x => x.ObjectToChoose == obj);

            if (chosenObjectModel == null)
            {
                ProcessWrongChoice(obj);
                return;
            }

            if (chosenObjectModel.ChoiceCompleted)
                return;

            CompleteChoice(chosenObjectModel);
        }
    }
}