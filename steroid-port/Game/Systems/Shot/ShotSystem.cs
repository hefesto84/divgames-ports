﻿using System;
using System.Collections.Generic;
using Raylib_cs;
using steroid_port.Game.Services;
using steroid_port.Game.Systems.Ship;
using steroid_port.Game.Views;

namespace steroid_port.Game.Systems.Shot
{
    public class ShotSystem : Base.System
    {
        private readonly ScreenService _screenService;
        private readonly SpriteService _spriteService;
        private readonly RenderService _renderService;
        private readonly ShipSystem _shipSystem;

        private Queue<ShotView> _views;
        private List<ShotView> _currentUsedViews;
        private List<ShotView> _toRecycle;

        private int _maxShots = 10;

        public List<ShotView> Shots => _currentUsedViews;
        
        public ShotSystem(ScreenService screenService, SpriteService spriteService, RenderService renderService, ShipSystem shipSystem)
        {
            _screenService = screenService;
            _spriteService = spriteService;
            _renderService = renderService;
            _shipSystem = shipSystem;
        }

        public override void Init()
        {
            Reset();
        }

        public override void Reset()
        {
            _views ??= new Queue<ShotView>(_maxShots);
            _currentUsedViews ??= new List<ShotView>(_maxShots);
            _toRecycle ??= new List<ShotView>(_maxShots);

            _views.Clear();
            _currentUsedViews.Clear();

            SetupShotView();
        }

        public override void Update()
        {
            Recycle();
            
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_F))
            {
                Shoot();
            }

            var list = _currentUsedViews.GetEnumerator();

            while (list.MoveNext())
            {
                list.Current?.UpdateView();
            }
            
            list.Dispose();
        }

        private void Recycle()
        {
            foreach (var t in _currentUsedViews)
            {
                if(!t.IsReady) continue;
                _toRecycle.Add(t);
            }

            foreach (var t in _toRecycle)
            {
                _currentUsedViews.Remove(t);
                _views.Enqueue(t);
            }
            
            _toRecycle.Clear();
        }
        
        private void Shoot()
        {
            if (_views.Count != 0)
            {
                var v = _views.Dequeue();
                v.SetView(_shipSystem.CurrentPosition, _shipSystem.CurrentVelocity, _shipSystem.CurrentRotation);
                v.OnOutOfScreen = OnShotReadyToBeUsed;
                _currentUsedViews.Add(v);
            }
        }
        
        private void SetupShotView()
        {
            for (var i = 0; i < 10; i++)
            {
                var view = new ShotView(_renderService, _screenService);
                view.Init(_spriteService);
                _views.Enqueue(view);
            }
        }

        public void OnShotReadyToBeUsed(ShotView shotView)
        {
            //_toRecycle.Add(shotView);
        }
    }
}