﻿using System;
using Raylib_cs;
using steroid_port.Game.Configurations;
using steroid_port.Game.Factories;
using steroid_port.Game.Managers;
using steroid_port.Game.Services;
using steroid_port.Game.States;
using steroid_port.Game.States.Base;
using steroid_port.Game.Systems.Background;
using steroid_port.Game.Systems.Render;
using steroid_port.Game.Systems.Ship;
using steroid_port.Game.Systems.UI;
using steroid_port.Game.Utils;

namespace steroid_port.Game
{
    public class Bootstrap
    {
        private GameManager _gameManager;
        private StateFactory _stateFactory;
        private ConfigService _configService;
        private RenderService _renderService;
        private ScreenService _screenService;
        private SpriteService _spriteService;
        private Utilities _utilities;

        private ShipSystem _shipSystem;
        private BackgroundSystem _backgroundSystem;
        private RenderSystem _renderSystem;
        private UISystem _uiSystem;
        
        public bool IsQuit => Raylib.WindowShouldClose();

        private readonly SteroidConfig _steroidConfig;

        public Bootstrap(SteroidConfig steroidConfig)
        {
            _steroidConfig = steroidConfig;
        }

        public void Init()
        {
            _gameManager = new GameManager();

            InitUtilities();
            InitServices();
            BuildSystems();
            InitFactories();
            
            _gameManager.Init(_stateFactory);
            _gameManager.SetState(_stateFactory.Get(StateType.InitGameState));
        }
        
        public void Update()
        {
            _renderService.Begin();
            
            _gameManager.Update();
            
            _renderService.End();
        }

        private void InitUtilities()
        {
            _utilities = new Utilities();
        }
        
        private void InitServices()
        {
            _configService = new ConfigService();
            _renderService = new RenderService(_steroidConfig);
            _screenService = new ScreenService();
            _spriteService = new SpriteService(_screenService);
            
            
            _configService.Init(_steroidConfig);
            _screenService.Init(_steroidConfig.Width,_steroidConfig.Height);
            _renderService.Init();
            _spriteService.Init();
        }
        
        private void BuildSystems()
        {
            _shipSystem = new ShipSystem(_screenService, _spriteService, _renderService);
            _backgroundSystem = new BackgroundSystem(_spriteService, _renderService);
            _renderSystem = new RenderSystem();
            _uiSystem = new UISystem(_configService, _screenService, _utilities);
        }
        
        private void InitFactories()
        {
            _stateFactory = new StateFactory();
            _stateFactory.Init();
            _stateFactory.RegisterState(new InitGameState(_gameManager, _uiSystem, _backgroundSystem, StateType.InitGameState));
            _stateFactory.RegisterState(new GameState(_gameManager, _backgroundSystem, _shipSystem, StateType.GameState));
            _stateFactory.RegisterState(new GameOverState(_gameManager, StateType.GameOverState));
        }

        ~Bootstrap()
        {
            Console.WriteLine("Destroy here EVERYTHING");
        }
    }
}