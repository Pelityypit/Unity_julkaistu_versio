using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System.Linq;
using CodeMonkey;
using UnityEngine.UI;

public class Snake : MonoBehaviour
{
    [SerializeField] ParticleSystem _particleSystem;

    private enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }
    //käärmeen state = elävä tai kuollut
    public enum State
    {
        Alive,
        Dead
    }
    private State state;
    public Vector2Int gridPosition; // määrittelee käärmeen sijainnin
    private Direction gridMoveDirection; // määrittelee automaattisen liikkeen suunnan
    private float gridMoveTimer; // määrittelee ajan seuraavaan liikeeseen
    public float gridMoveTimerMax; // määrittelee ajan liikkeiden välillä, eli käärmeen liikkumisnopeuden
    private LevelGrid levelGrid;
    private int snakeBodySize; // muuttuja jossa tallennetaan käärmeen koko
    private List<SnakeMovePosition> snakeMovePositionList; // lista johon tallennetaan käärmeen kasvu ja sen osat
    private List<SnakeBodyPart> snakeBodyPartList; // lista johon tallennetaan käärmeen kasvu muutokset
    public Vector2Int snakeBodyPartGridPosition;

    // GameObjectit
    public GameObject speedBoostObj;
    public GameObject escapeDeathObj;
    public GameObject bombObj;
    public GameObject spawnFoodObj;
    public GameObject gameHandler;

    public bool snakeAteSpeedBoost;
    public bool snakeAteEscapeDeath;
    public bool escapeDeathActive;
    public bool snakeAteBomb;

    public void Setup(LevelGrid levelGrid)
    {
        this.levelGrid = levelGrid;
    }
    private void Awake()
    {
        gridPosition = new Vector2Int(0, 0); // asetetaan sijainti: x=10, y=10
        gridMoveTimerMax = 0.2f; // liikkumisnopeus: mitä pienempi arvo, sitä useampi liike per frame
        gridMoveTimer = gridMoveTimerMax;
        gridMoveDirection = Direction.Right;  // alustetaan käärme liikkumaan oikealle
        snakeMovePositionList = new List<SnakeMovePosition>();
        snakeBodySize = 0; // alustetaan käärmeen kooksi 0
        snakeBodyPartList = new List<SnakeBodyPart>();
        //Alustetaan käärme eläväksi pelin alkaessa
        state = State.Alive;
    }

    public void Update()
    {
        switch (state)
        {
            case State.Alive:
                HandleInput();
                HandleGridMovement();
                break;
            //Jos state muuttuu kuolleeksi: peli päättyy
            case State.Dead:
                // Kun käärme kuolee astetaan gameobjektit ei aktiivisiksi
                bombObj.SetActive(false);
                escapeDeathObj.SetActive(false);
                speedBoostObj.SetActive(false);
                break;
        }
    }
    // -- LIIKKUMINEN --
    // määritetään liikkumisnapit: wasd ja nuolinapit
    // määritetään liikkumissuunnat, asetetaan x ja y-askeleille arvot suunnan mukaisesti
    // ylöspäin liikkuessa x=0 ja y=+1 koska liikutaan vertikaalisesti mutta ei horisontaalisesti

    private void HandleInput()
    {
        // Esc painettaessa pysäytetään peli
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameHandler.GamePaused(true);
        }
        // Uudestaan painettaessa esc jatketaan peliä
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameHandler.GamePaused(false);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (gridMoveDirection != Direction.Down && gridMoveDirection != Direction.Up)
            { // jos emme liiku alaspäin, voimme liikkua ylöspäin
                gridMoveDirection = Direction.Up;
                SoundManager.PlaySound(SoundManager.Sound.SnakeTurn);
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (gridMoveDirection != Direction.Up && gridMoveDirection != Direction.Down)
            {
                gridMoveDirection = Direction.Down;
                SoundManager.PlaySound(SoundManager.Sound.SnakeTurn);
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (gridMoveDirection != Direction.Right && gridMoveDirection != Direction.Left)
            {
                gridMoveDirection = Direction.Left;
                SoundManager.PlaySound(SoundManager.Sound.SnakeTurn);
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (gridMoveDirection != Direction.Left && gridMoveDirection != Direction.Right)
            {
                gridMoveDirection = Direction.Right;
                SoundManager.PlaySound(SoundManager.Sound.SnakeTurn);
            }
        }
    }
    // -- KENTÄN PÄIVITYS -- 
    public void HandleGridMovement()
    {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;
            SnakeMovePosition previousSnakeMovePosition = null;
            //jos on olemassa aikaisempi kehon sijainti niin tallennetaan se uuteen kehon osaan
            if (snakeMovePositionList.Count > 0)
            {
                previousSnakeMovePosition = snakeMovePositionList[0];
            }

            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previousSnakeMovePosition, gridPosition, gridMoveDirection);
            // tallennetaan listaan käärmeen kehon sijainti
            snakeMovePositionList.Insert(0, snakeMovePosition);

            //Liikkuvuuden määritys
            Vector2Int gridMoveDirectionVector;
            switch (gridMoveDirection)
            {
                default:
                case Direction.Right: gridMoveDirectionVector = new Vector2Int(+1, 0); break;
                case Direction.Left: gridMoveDirectionVector = new Vector2Int(-1, 0); break;
                case Direction.Up: gridMoveDirectionVector = new Vector2Int(0, +1); break;
                case Direction.Down: gridMoveDirectionVector = new Vector2Int(0, -1); break;
            }
            //muutetaaan gridPosition
            gridPosition += gridMoveDirectionVector;
            bool snakeAteFood = spawnFoodObj.GetComponent<SpawnFood>().TrySnakeEatFood(gridPosition);

            if (snakeAteFood)
            {
                // kun käärme syö, kasvata kehoa
                snakeBodySize++;
                CreateSnakeBodyPart(); // kun kasvu tapahtuu tee keho
                SoundManager.PlaySound(SoundManager.Sound.SnakeEatFruit); // Syömisen ääniefekti
            }
            // Koska gameobjektit tuhotaan omissa scripteissään, pitää ne asettaa uudelleen aktiiviseksi että ne päivittyy
            if (bombObj != null)
            {
                bombObj.SetActive(true);
                snakeAteBomb = bombObj.GetComponent<SpawnBomb>().TrySnakeEatBomb(gridPosition);
            }
            if (speedBoostObj != null)
            {
                speedBoostObj.SetActive(true);
                snakeAteSpeedBoost = speedBoostObj.GetComponent<SpawnSpeedBoost>().TrySnakeEatSpeedBoost(gridPosition);
            }
            if (escapeDeathObj != null)
            {
                escapeDeathObj.SetActive(true);
                snakeAteEscapeDeath = escapeDeathObj.GetComponent<SpawnEscapeDeath>().TrySnakeEatEscapeDeath(gridPosition);
            }
            // testataan onko lista liian iso perustuen käärmeen kokooon
            if (snakeMovePositionList.Count >= snakeBodySize + 1)
            { // jos listassa on yksi ylimääräinen osa
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1); // poistetaan listasta ylimääräinen osa
            }

            UpdateSnakeBodyParts();
            //käydään läpi kaikki kehon osat
            for (int i = 0; i < snakeBodyPartList.Count(); ++i)
            {
                snakeBodyPartGridPosition = snakeBodyPartList[i].GetGridPosition();
                escapeDeathActive = escapeDeathObj.GetComponent<SpawnEscapeDeath>().isEscapeDeathActive;

                if (escapeDeathActive && gridPosition == snakeBodyPartGridPosition)
                {
                    // Ei kuole törmäyksessä
                }
                else if (gridPosition == snakeBodyPartGridPosition) //jos käärmeen pään sijainti on sama kuin jollain sen kehon osalla
                {
                    SoundManager.PlaySound(SoundManager.Sound.SnakeDeath); //Kuoleman ääni
                    state = State.Dead;
                    // Kun käärme kuolee particle system käynnistyy
                    _particleSystem.Play();
                    //Kutsutaan GameHandlerin SnakeDied funktiota
                    gameHandler.GetComponent<GameHandler>().SnakeDied();
                }
            }
            // päivitetään käärmeen sijainti gridPositionin x ja y arvoilla
            transform.position = new Vector3(gridPosition.x, gridPosition.y);
            // haetaan pään kääntyminen Vector2Intin kulmasta (z-akseli), joka saa parametrina pään suunnan
            // euler angle on oikea termi z-akselille
            // kulmasta pitää vähentää 90 astetta, koska unityssä 0-arvo osoittaa oikealle
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 90);
            // Käärme liikkuu ruudun toiselle puolelle
            gridPosition = levelGrid.ValidateGridPosition(gridPosition);
        }
    }
    // luo käärmeen kehon käärmeen syötyä hedelmän
    private void CreateSnakeBodyPart()
    {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count));
    }
    // lisää kehoon lisää palasia
    private void UpdateSnakeBodyParts()
    {
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            snakeBodyPartList[i].SetSnakeMovePosition(snakeMovePositionList[i]);
        }
    }
    // määritetään pään kulma kääntyessä
    private float GetAngleFromVector(Vector2Int dir)
    {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }
    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }
    // Palauttaa listan käärmeen pään ja kehon sijainneista
    public List<Vector2Int> GetFullSnakeGridPositionList()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition };
        foreach (SnakeMovePosition snakeMovePosition in snakeMovePositionList)
        {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }
        return gridPositionList;
    }
    private class SnakeBodyPart
    {
        private SnakeMovePosition snakeMovePosition;
        private Transform transform;
        // luodaan uusi gameObject "SnakeBody" 
        // lisätään uusi snakebody sprite GameScene/GameHandler/GameAssets
        public SnakeBodyPart(int bodyIndex)
        {
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeBodySprite;
            // kehon osien lisäys tapahtuu käärmeen kehon häntäpäädyssä
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -1;
            transform = snakeBodyGameObject.transform;
        }
        // kehon sijainnin määritys
        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition)
        {
            this.snakeMovePosition = snakeMovePosition;
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);
            //kehon kulman määritys käännyttäessä
            float angle;
            switch (snakeMovePosition.GetDirection())
            {
                default:
                case Direction.Up: //kun kääntyy ylös
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 0; break; //ylöspäin mentäessä käärmeen kulma on 0 astetta
                        case Direction.Left: //liikkuessa vasemmalle
                            angle = 20; //kehon osan asteet käännöksessä
                            transform.position += new Vector3(.2f, .2f);
                            break;
                        case Direction.Right: //liikkuessa oikealle
                            angle = -25; //kehon osan asteet käännöksessa
                            transform.position += new Vector3(-.2f, .2f);
                            break;
                    }
                    break;
                case Direction.Down: //kun kääntyy alas
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 180; break; //alaspäin mentäessä käärmeen kulma on 180 astetta
                        case Direction.Left: //liikkuessa vasemmalle
                            angle = 180 - 25;
                            transform.position += new Vector3(.2f, -.2f);
                            break;
                        case Direction.Right: //liikkuessa oikealle
                            angle = 180 + 15;
                            transform.position += new Vector3(-.2f, -.2f);
                            break;
                    }
                    break;
                case Direction.Left: //kun kääntyy vasemmalle
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 90; break; //vasemmalle liikkuessa käärmeen kulma on 90 astetta
                        case Direction.Down: //liikkuessa alaspäin
                            angle = 90 + 20;
                            transform.position += new Vector3(-.2f, .2f);
                            break;
                        case Direction.Up: //liikkuessa ylöspäin
                            angle = 90 - 25;
                            transform.position += new Vector3(-.2f, -.2f);
                            break;
                    }
                    break;
                case Direction.Right: //kun kääntyy oikealle
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = -90; break; //oikealle liikkuessa käärmeen kulma on -90 astetta
                        case Direction.Down: //liikkuessa alaspäin
                            angle = -115;
                            transform.position += new Vector3(.2f, .2f);
                            break;
                        case Direction.Up: //liikkuessa ylöspäin
                            angle = -75;
                            transform.position += new Vector3(.2f, -.2f);
                            break;
                    }
                    break;
            }
            transform.eulerAngles = new Vector3(0, 0, angle);
        }
        //palautetaan käärmeen gridposition
        public Vector2Int GetGridPosition()
        {
            return snakeMovePosition.GetGridPosition();
        }
    }
    //Tallentaa käärmeen kehon osan liikeen suunnan sekä sijainnin ja jakaa sen eteenpäin
    private class SnakeMovePosition
    {
        //aikaisemman kehon osan sijainti
        private SnakeMovePosition previousSnakeMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;

        //kehon osien liikesuuntien ja sijaintien konstruktori
        public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition, Vector2Int gridPosition, Direction direction)
        {
            this.previousSnakeMovePosition = previousSnakeMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }
        public Vector2Int GetGridPosition()
        {
            return gridPosition;
        }
        public Direction GetDirection()
        {
            return direction;
        }
        public Direction GetPreviousDirection()
        {
            //Jos ei ole aikaisempaa liikesuuntaa, palauttaa käärmeelle vakio liikesuunnan
            if (previousSnakeMovePosition == null)
            {
                return Direction.Right;
            }
            else
            {
                return previousSnakeMovePosition.direction;
            }
        }
    }
}
