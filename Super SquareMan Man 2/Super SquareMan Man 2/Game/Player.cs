using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace SSMM2.Game
{
	public class PlayerHealthDisplay : Core.Entity
	{
		Texture2D m_EmptyHealthSprite, m_HealthSprite;

		public int SpriteSpacing = 10;

		int m_MaxHealthCount = 0;
		public int MaxHealthCount
		{
			get
			{
				return m_MaxHealthCount;
			}
			set
			{
				m_MaxHealthCount = value;
				if (m_MaxHealthCount < 0)
					m_MaxHealthCount = 0;
				if (CurrentHealthCount > m_MaxHealthCount)
					CurrentHealthCount = m_MaxHealthCount;
			}
		}

		int m_CurrentHealthCount = 0;
		public int CurrentHealthCount
		{
			get
			{
				return m_CurrentHealthCount;
			}

			set
			{
				m_CurrentHealthCount = value;
				if (m_CurrentHealthCount < 0) m_CurrentHealthCount = 0;
				if (m_CurrentHealthCount > MaxHealthCount) m_CurrentHealthCount = MaxHealthCount;
			}
		}

		public PlayerHealthDisplay(Core.Scene ownerScene)
			: base (ownerScene)
		{
		}

		public override void LoadContent()
		{
			var resource = Core.ResourceManager.Instance;
			m_EmptyHealthSprite = resource.GetResource<Texture2D>("emptyhealth");
			m_HealthSprite = resource.GetResource<Texture2D>("filledhealth");

			Depth = -50000;
		}

		public override void Draw(SpriteBatch spriteBatch, Core.PrimitiveBatch primitiveBatch)
		{
			Texture2D drawSprite = null;
			Vector2 drawPosition;

			int drawOffset = 0;

			for (int i = 0; i < MaxHealthCount; i++)
			{
				drawPosition = Vector2.Transform(Position + new Vector2(drawOffset, 0.0F), OwnerScene.Camera.ScreenToWorldMatrix);

				if (i < CurrentHealthCount)
				{
					drawSprite = m_HealthSprite;
				}
				else
				{
					drawSprite = m_EmptyHealthSprite;
				}

				drawOffset += drawSprite.Width + SpriteSpacing;

				spriteBatch.Draw(drawSprite, drawPosition, BlendColor);
			}
		}
	}

	public class Player : Core.KinematicEntity, CollisionType.Player, GunHolder
	{
		Gun m_Gun = null;
		public Gun Gun
		{
			get
			{
				return m_Gun;
			}
			set
			{
				if (m_Gun != null)
					m_Gun.Owner = null;

				m_Gun = value;
				m_Gun.Owner = this;
			}
		}

		private RespawnPoint m_PreviousRespawn = null;
		private PlayerHealthDisplay m_HealthDisplay = null;

		public bool CanSlowTime = false;
		private bool m_IsSlowTime = false;

		public Vector2 SpawnPosition;

		public int MaxHealth;
		public int Health;

		public int JumpsRemaining
		{
			get;
			private set;
		}

		public int TotalJumpsAllowed
		{
			get;
			internal set;
		}

		public Player(Core.Scene ownerScene)
			: base (ownerScene)
		{
			Core.InputManager input = Core.InputManager.Instance;
			input.Bind("left", Keys.Left);
			input.Bind("left", Keys.A);
			input.Bind("right", Keys.Right);
			input.Bind("right", Keys.D);

			input.Bind("jump", Keys.Up);
			input.Bind("jump", Keys.W);
			input.Bind("jump", Keys.Space);

			input.Bind("slow time", Keys.E);

			BlendColor = Color.Red;

			Core.DataStore.Context["Player"] = this;

			TotalJumpsAllowed = 1;
			JumpsRemaining = 0;

			MaxHealth = 8;
			Health = MaxHealth;

			TimeWarpController.Spawn(ownerScene);
		}

		public override void LoadContent()
		{
			SetSpriteFromResource("Player");

			Depth = -5;

			SpawnPosition = Position;

			//Guns.Pistol pistol = new Guns.Pistol(OwnerScene, this);
			//m_Gun = pistol;
			
			m_HealthDisplay = new PlayerHealthDisplay(OwnerScene);
		}

		public Vector2 GunHoldingPosition
		{
			get
			{
				return Position;
			}
		}

		public Core.ColorScheme.SchemeType ColorScheme
		{
			get
			{
				return Core.ColorScheme.SchemeType.Good;
			}
		}

		public override void Update(float timeDelta)
		{
			base.Update(timeDelta);


			if (null != OwnerScene.CollisionWorld.EntityPlaceFree<OutsideRoomBorder>(this, Position))
				Health = 0;



			Core.InputManager input = Core.InputManager.Instance;
			uint authKey = Core.InputManager.DefaultAuthKey;

			if (input.CheckPressed("slow time", authKey) && CanSlowTime)
			{
				m_IsSlowTime = !m_IsSlowTime;

				if (m_IsSlowTime)
				{
					TimeWarpController.Instance.TargetTimeScale = 0.2F;
				}
				else
				{
					TimeWarpController.Instance.TargetTimeScale = 1.0F;
				}
			}

			if (input.Check("left", authKey))
				ApplyForceX(-1000.0F, float.PositiveInfinity);
			if (input.Check("right", authKey))
				ApplyForceX(1000.0F, float.PositiveInfinity);

			if (IsGrounded())
				JumpsRemaining = TotalJumpsAllowed;
			else
			{
				if (JumpsRemaining == TotalJumpsAllowed)
					JumpsRemaining = TotalJumpsAllowed - 1;
			}

			if (input.CheckPressed("jump", authKey) && JumpsRemaining > 0)
			{
				Velocity.Y = -800.0f;
				JumpsRemaining--;
			}

			Core.Camera camera = OwnerScene.Camera;
			camera.BorderedAbsoluteFollow(Position);

			if (m_Gun != null)
			{
				Vector2 mousePosition;
				MouseState mouse = Core.InputManager.Instance.GetMouse(Core.InputManager.DefaultAuthKey);
				mousePosition = new Vector2(
					mouse.X,
					mouse.Y
					);
				mousePosition = Vector2.Transform(mousePosition, OwnerScene.Camera.ScreenToWorldMatrix);
				m_Gun.AimAt(mousePosition);

				if (mouse.LeftButton == ButtonState.Pressed)
				{
					m_Gun.ShootAt(mousePosition);
				}
			}

			//Debug.DebugView debugView = Debug.DebugView.Context;
			//debugView.DisplayText("Velocity: " + Velocity.ToString());
			//debugView.DisplayText("Position: " + Position.ToString());

			Debug.DebugView.Context.UpdateOutputSlot("Player Health", Health.ToString());

			if (Health <= 0)
			{
				Position = SpawnPosition;
				Health = MaxHealth;

				Core.SceneManager.Instance.PushModifier(false, false);
				Core.SceneManager.Instance.PushScene(UI.GameOverMenu.CreateNewGameOverMenuScene());
			}

			HandlePowerupChecks();
			HandleCheckpointChecks();

			m_HealthDisplay.CurrentHealthCount = Health;
			m_HealthDisplay.MaxHealthCount = MaxHealth;
		}

		public void AddGun(Gun newGun)
		{
			if (Gun != null)
				Gun.Destroy();
			Gun = newGun;
			Effects.FloatingSpecialMessage.Spawn(OwnerScene, Position, "SWEET NEW " + Gun.Name.ToUpper());
		}

		public override void Hit(Core.Entity source)
		{
			if (source is Game.Bullet)
				Health -= 1;
		}

		private void HandlePowerupChecks()
		{
			var collision = OwnerScene.CollisionWorld;
			Core.Entity result = null;

			result = collision.EntityPlaceFree<CollisionType.Powerup>(this, Position);
			if (result != null)
			{
				var powerup = result as CollisionType.Powerup;
				powerup.Apply(this);

				String message = "AW YEAH, " + powerup.Name.ToUpper() + " UPGRADE";
				if (powerup.Instructions.Length != 0)
					message += "\n(" + powerup.Instructions + ")";

				Effects.FloatingSpecialMessage.Spawn(OwnerScene, Position, message);

				result.Destroy();
			}
		}

		private void HandleCheckpointChecks()
		{
			var collision = OwnerScene.CollisionWorld;
			Core.Entity result = collision.EntityPlaceFree<CollisionType.RespawnArea>(this, Position);
			if (result != null)
			{
				if (m_PreviousRespawn != result)
				{
					m_PreviousRespawn = result as RespawnPoint;
					SpawnPosition = Position;
					Effects.FloatingSpecialMessage.Spawn(OwnerScene, Position, "CHECKPOINT");
				}
			}
		}
	}
}
