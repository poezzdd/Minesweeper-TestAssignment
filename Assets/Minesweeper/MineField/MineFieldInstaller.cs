using Minesweeper.MineField.Realization;
using Zenject;

namespace Minesweeper.MineField
{
    public class MineFieldInstaller : Installer<MineFieldInstaller>
    {
        private const int CellsInitialSize = 100;
        private const string CellsContainerName = "Cells";
        
        public override void InstallBindings()
        {
            Container
                .Bind<MineFieldConfig>()
                .FromScriptableObjectResource(nameof(MineFieldConfig))
                .AsSingle();
            
            Container
                .BindMemoryPool<MineFieldCellView, MineFieldCellView.Pool>()
                .WithInitialSize(CellsInitialSize)
                .FromComponentInNewPrefabResource(nameof(MineFieldCellView))
                .UnderTransformGroup(CellsContainerName);

            Container
                .BindInterfacesAndSelfTo<MineFieldController>()
                .AsSingle();
        }
    }
}