using System;
using Minesweeper.Application.Realization;
using Minesweeper.MineField;
using Minesweeper.UI.Realization;
using UnityEngine;
using Zenject;

namespace Minesweeper.Application
{
    public class ApplicationInstaller : MonoInstaller<ApplicationInstaller>
    {
        [SerializeField] private GameSceneData gameSceneData;
        
        public override void InstallBindings()
        {
            Container
                .Bind<GameSceneData>()
                .FromInstance(gameSceneData)
                .AsSingle();
            
            MineFieldInstaller.Install(Container);
            
            Container
                .BindInterfacesAndSelfTo<GameTimer.Realization.GameTimer>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<UIGameWindowController>()
                .AsSingle()
                .NonLazy();
            
            Container
                .BindInterfacesAndSelfTo<GameStarter.Realization.GameStarter>()
                .AsSingle();

            Container
                .Bind<ApplicationStartup>()
                .AsSingle()
                .NonLazy();
        }
    }
    
    [Serializable]
    public class GameSceneData
    {
        public UIGameWindow UIGameWindow => uiGameWindow;
        public Transform FieldTransform => fieldTransform;
        
        [SerializeField] private UIGameWindow uiGameWindow;
        [SerializeField] private Transform fieldTransform; 
    }
}