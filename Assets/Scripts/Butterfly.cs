using UnityEngine;

namespace PixelRainbows.Prototyping
{
    public class Butterfly : MonoBehaviour
    {
        [SerializeField]
        protected Rigidbody2D body;

        float speed;
        ButterflyGame game;
        new Camera camera;

        public void Init(ButterflyGame bgame, float newSpeed, float initialRotation, Camera cam)
        {
            game = bgame;
            speed = newSpeed;
            body.rotation = initialRotation;
            camera = cam;
            body.isKinematic = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(game)
                game.EnterButtfly();
        }

        private void OnTriggerExit2D(Collider2D other) 
        {
            game.LeaveButtfly();
            print("exit");
        }

        private void FixedUpdate() 
        {
            body.rotation += RandomNormalDistribution(-5f, 5f);
            body.velocity = transform.up * speed;
        }

        //returns a value between min and max with a distribution that approximates the normal distribution.
        float RandomNormalDistribution(float min, float max)
        {
            min /= 2f; max /= 2f;
            return Random.Range(min, max) + Random.Range(min, max);
        }

        private void OnMouseDown()
        {
            body.isKinematic = true;
        }

        private void OnMouseDrag() 
        {
            Vector2 pos = camera.ScreenToWorldPoint(Input.mousePosition);
            body.position = pos;
        }

        private void OnMouseUp() 
        {
            body.isKinematic = false;
        }

        public void Stop()
        {
            body.isKinematic = true;
        }
    }
}