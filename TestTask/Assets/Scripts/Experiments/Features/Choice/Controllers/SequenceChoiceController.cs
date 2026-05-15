using System.Linq;
using Features.Choice.Components;
using Features.Choice.Models;

namespace Features.Choice.Controllers
{
    public class SequenceChoiceController : ChoiceController
    {
        protected override void OnObjectClick(ChoiceComponent obj)
        {
            ChoiceModel chosenObjectModel = 
                objectsToChoose.FirstOrDefault(x => !x.ChoiceCompleted);

            if (chosenObjectModel == null || chosenObjectModel.ObjectToChoose != obj)
            {
                ProcessWrongChoice(obj);
                return;
            }

            CompleteChoice(chosenObjectModel);
        }
    }
}