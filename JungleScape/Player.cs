﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace JungleScape
{
    public class Player : Character
    {
        // attributes
        const int MAX_FALL_SPEED = -10;
        Rectangle leftSide;
        Rectangle rightSide;
        Rectangle topSide;
        Rectangle bottomSide;
        KeyboardState keyState;
        enum AimDirection { Forward, Diagonal, Up };
        AimDirection aimDirection;

        // constructor
        public Player(Rectangle hBox, Texture2D texture) : base(hBox, texture)
        {
            keyState = new KeyboardState();
            aimDirection = AimDirection.Forward;
            speedX = 5;
            speedY = 0;

            // set rectangles for collsion detection
            leftSide = new Rectangle(hitBox.X, hitBox.Y, -3, hitBox.Height);
            rightSide = new Rectangle((hitBox.X + hitBox.Width), hitBox.Y, 3, hitBox.Height);
            topSide = new Rectangle(hitBox.X, hitBox.Y, hitBox.Width, -3);
            bottomSide = new Rectangle(hitBox.X, (hitBox.Y + hitBox.Height), hitBox.Width, 3);
        }

        // methods
        public override void Move(List<GameObject> platforms)
        {
            keyState = Keyboard.GetState();

            // use IsKeyDown to determine if a partuclar key is being pressed. Use 4 if statesments for wasd
            // if the top of the player isn't intersecting any platforms, and the bottom of the player is intersecting the platform, run jump logic
            if (PlayerDetectCollision(bottomSide, platforms))
            {
                // first, set speedy to 0, player should no be moving in y direction when on a platform with no key press
                speedY = 0;

                // check if the player is colliding with a platform above them
                if (!PlayerDetectCollision(topSide, platforms))
                {
                    // Allow jump if these conditions are met.
                    if (keyState.IsKeyDown(Keys.W))
                    {
                        speedY = 10;
                        hitBox.Y -= speedY;
                        speedY--;
                    }
                }
            }
            else
            {
                hitBox.Y -= speedY;
                speedY--;
            }

            // if the left side of the player does not instersect any platforms, move the player to the left.
            if (!PlayerDetectCollision(leftSide, platforms))
            {
                if (keyState.IsKeyDown(Keys.A))
                {
                    hitBox.X -= speedX;
                }
            }

            // if the right side of the player does not interesect any platforms, move the player to the right.
            if (!PlayerDetectCollision(rightSide, platforms))
            {
                if (keyState.IsKeyDown(Keys.D))
                {
                    hitBox.X += speedX;
                }
            }
            
            // Maximum falling speed. If exceeded, resets the falling speed to the maximum. Ensures not continuous accelleration
            if (speedY < MAX_FALL_SPEED)
            {
                speedY = MAX_FALL_SPEED;
            }

            // TEMP CODE: ALLOW PLAYER TO MOVE DOWN WITH 'S' FOR TESTING
            if (keyState.IsKeyDown(Keys.S))
                hitBox.Y += 10;
        }

        // Aim will determine which direction the player in inputting to aim in
        public string Aim()
        {
            keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Up))
            {
                aimDirection = AimDirection.Up;
                return "up";
            }      
            if (keyState.IsKeyDown(Keys.Right))
            {
                aimDirection = AimDirection.Forward;
                return "right";
            }
            if (keyState.IsKeyDown(Keys.Left))
            {
                aimDirection = AimDirection.Forward;
                return "left";
            }
            if (keyState.IsKeyDown(Keys.Up) && keyState.IsKeyDown(Keys.Right))
            {
                aimDirection = AimDirection.Diagonal;
                return "diagonal right";
            }
            if (keyState.IsKeyDown(Keys.Up) && keyState.IsKeyDown(Keys.Left))
            {
                aimDirection = AimDirection.Diagonal;
                return "diagonal left";
            }
            else
                return null;
        }

        // FireArrow method will create an arrow with speed based on the direction passed in by Aim. Requires the image for the arrow be passed in.
        public void FireArrow(Texture2D arrowImage, List<GameObject> objects)
        {
            // get what direction the player is aiming in
            string direction = Aim();
            int timer = 0;

            // check to see if the player has pressed spacebar to fire
            if (keyState.IsKeyDown(Keys.Space) && timer >= 60)
            {
                if (direction == "up")
                {
                    // creates an arrow, 0 horizontal speed, 8 verticle, starts in player center with dimesnions 20x5, and uses the passed in image
                    Arrow arrow = new Arrow(0, -8, new Rectangle(hitBox.Center, new Point(20, 5)), arrowImage);

                    // make the arrow move 
                    arrow.Move(objects);

                    // reset timer
                    timer = 0;
                }
                if (direction == "right")
                {
                    Arrow arrow = new Arrow(8, 0, new Rectangle(hitBox.Center, new Point(20, 5)), arrowImage);
                    arrow.Move(objects);
                    timer = 0;
                }
                if (direction == "left")
                {
                    Arrow arrow = new Arrow(-8, 0, new Rectangle(hitBox.Center, new Point(20, 5)), arrowImage);
                    arrow.Move(objects);
                    timer = 0;
                }
                if (direction == "diagonal right")
                {
                    Arrow arrow = new Arrow(4, -4, new Rectangle(hitBox.Center, new Point(20, 5)), arrowImage);
                    arrow.Move(objects);
                    timer = 0;
                }
                if (direction == "diagonal left")
                {
                    Arrow arrow = new Arrow(-4, -4, new Rectangle(hitBox.Center, new Point(20, 5)), arrowImage);
                    arrow.Move(objects);
                    timer = 0;
                }
            }

            // code to stop rapid fire arrows here
            timer++;
        }

        // specialized detect collision for each side of the player.
        private bool PlayerDetectCollision(Rectangle side, List<GameObject> platforms)
        {
            if (platforms.Count != 0)
            {
                foreach (GameObject platform in platforms)
                {
                    if (side.Intersects(platform.hitBox))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
                return false;
        }

        // Original Move. Not being used.
        public override void Move()
        {
        }
    }
}

