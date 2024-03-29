﻿using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;
    public Text prob1message;
    public Text dineroBanca;
    public Text dineroApuesta;
    public int apuesta;
    public int banca = 1000;

    public int[] values = new int[52];
    int cardIndex = 0;   
    private void Awake()
    {    
        InitCardValues();  
    }

    private void Start()
    {
        ShuffleCards();
        StartGame();    
        dineroBanca.text = banca.ToString();    
    }

    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */
        int contador = 0;
        for(int palo = 1; palo <= 4; palo++){
            for(int carta = 1; carta <=13; carta++){
                if(carta <=10){
                    values[contador] = carta;
                } else if(carta > 10){
                    values[contador] = 10;
                }
                contador++;
            }
        }
    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */ 
        for(int i = 0; i<values.Length; i++){
            int position = Random.Range(i,values.Length);
            //guardo los valores
            Sprite auxS = faces[i];
            int aux = values[i];
            //invierto los valores de faces
            faces[i] = faces[position];
            faces[position] = auxS;
            //invierto los valores de values
            values[i] = values[position];
            values[position] = aux;
        }
    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
            //Si el dealer obtiene blackjack
            if(dealer.GetComponent<CardHand>().points == 21){
                //dar la vuelta a la carta del dealer
                dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
                //mostrar mensaje
                finalMessage.text = "Blackjack del dealer. Has perdido";
                //desactivar botones
                stickButton.interactable = false;
                hitButton.interactable = false;
                //banca
                banca -= apuesta;
                apuesta = 0;
                dineroBanca.text = banca.ToString(); 
                dineroApuesta.text = apuesta.ToString();
            }
            //si el player obtiene blackjack
            else if(player.GetComponent<CardHand>().points == 21){
                //dar la vuelta a la carta del dealer
                dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
                //mostrar mensaje
                finalMessage.text = "Blackjack. Has ganado";
                //desactivar botones
                stickButton.interactable = false;
                hitButton.interactable = false;
                //banca
                banca += apuesta *2;
                apuesta = 0;
                dineroApuesta.text = apuesta.ToString();
                dineroBanca.text = banca.ToString();
            }
        }
    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */
                
        float probabilities;
        int casosFavorables;
        
        int valorDealerTotal = dealer.GetComponent<CardHand>().points;
        int valorJugadorTotal = player.GetComponent<CardHand>().points;

        //probabilidad 1
        //inicio de partida
        if(player.GetComponent<CardHand>().cards.Count == 2){
            //carta descubierta, values[1] es la primera carta que se le repartió al dealer
            int valorDealerSinSecreto = valorDealerTotal - values[1];
            // numero de cartas de cada palo - puntos jugador - valor de las cartas descubiertas
            casosFavorables = 13 - valorJugadorTotal + valorDealerSinSecreto;
            //entonces la probabilidad será los casos favorables/numero de cartas de cada palo
            probabilities = casosFavorables / 13f;
            //redondeo de probabilidad
            if(probabilities > 1){
                probabilities = 1;
            } else if(probabilities< 0){
                probabilities = 0;
            }
            Debug.Log("entro");
            prob1message.text = "El dealer tenga más puntuación que el jugador en su carta oculta: " + (Mathf.Round(probabilities * 100)).ToString() + "%"; 
        } else{
            prob1message.text = "Ya no tiene carta oculta el dealer"; 
        }
        //probabilidad 2 y 3
        float casosTotales = 49;
        int casosFavorables2 = 0;
        int casosFavorables3 = 0;
        //empezamos la variable en este valor ya que ya conocemos los anteriores sucesos
        for (int i = player.GetComponent<CardHand>().cards.Count + dealer.GetComponent<CardHand>().cards.Count + 1; i < values.Length; i++)
        {
            //PARA SABER SI OBTENDRÁ ENTRE UN 17 Y UN 21 SI PIDE UNA CARTA
            if (values[i] + valorJugadorTotal <= 21 && values[i] + valorJugadorTotal >= 17)
            {
                casosFavorables2++;
            }
            //PARA SABER SI SE PASA DE 21
            if (values[i] + valorJugadorTotal > 21)
            {
                casosFavorables3++;
            }
        }
        probMessage.text = "El jugador obtenga entre un 17 y un 21 si pide una carta: " + (Mathf.Round((casosFavorables2/casosTotales) * 100)).ToString() + "%" +"\r\n" + "\r\n" + 
        "El jugador obtenga más de 21 si pide una carta: " + (Mathf.Round((casosFavorables3/casosTotales)*100)).ToString() + "%";

    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardIndex],values[cardIndex]);
        cardIndex++;        
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        CalculateProbabilities();
    }       

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        if (dealer.GetComponent<CardHand>().cards.Count == 2)
        {
            dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true); 
        }
        //Repartimos carta al jugador
        PushPlayer();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */      
        Debug.Log(dealer.GetComponent<CardHand>().points);
        //Si el dealer obtiene blackjack o el jugador obtiene mas de 21 puntos
        if(dealer.GetComponent<CardHand>().points == 21 || player.GetComponent<CardHand>().points > 21){
            //dar la vuelta a la carta del dealer
            dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
            //mostrar mensaje
            if(dealer.GetComponent<CardHand>().points == 21){
                finalMessage.text = "Blackjack del dealer. Has perdido";
            } else{
                finalMessage.text = "Has perdido";
            }
            stickButton.interactable = false;
            hitButton.interactable = false;
            //banca
            banca -= apuesta;
            apuesta = 0;
            dineroBanca.text = banca.ToString(); 
            dineroApuesta.text = apuesta.ToString();
        }
    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        if(dealer.GetComponent<CardHand>().cards.Count == 2){
            dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
        }
        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */                
        while(dealer.GetComponent<CardHand>().points <= 16){
            PushDealer();
        }
        if(dealer.GetComponent<CardHand>().points >= 17){
            if(dealer.GetComponent<CardHand>().points > player.GetComponent<CardHand>().points){
                finalMessage.text = "Has perdido";
                stickButton.interactable = false;
                hitButton.interactable = false;
                //banca
                banca -= apuesta;
                apuesta = 0;
                dineroBanca.text = banca.ToString(); 
                dineroApuesta.text = apuesta.ToString();
            } else if(player.GetComponent<CardHand>().points > dealer.GetComponent<CardHand>().points){
                finalMessage.text = "Has ganado";
                stickButton.interactable = false;
                hitButton.interactable = false;
                //banca
                banca += apuesta *2;
                apuesta = 0;
                dineroApuesta.text = apuesta.ToString();
                dineroBanca.text = banca.ToString();
            } else{
                finalMessage.text = "Empate";
                stickButton.interactable = false;
                hitButton.interactable = false;
                //banca
                apuesta = 0;
                dineroApuesta.text = apuesta.ToString();
            }
        }
    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        apuesta = 0;
        ShuffleCards();
        StartGame();
    }
    
    public void SubirApuesta(){
        int apuestax = apuesta + 10;
        if(apuestax > banca){
            apuesta = banca;
            dineroApuesta.text = apuesta.ToString();
        } else{
            apuesta += 10;
            dineroApuesta.text = apuesta.ToString();
        }
    }
    public void BajarApuesta(){
        int apuestax = apuesta - 10;
        if(apuestax < 0){
            apuesta = 0;
            dineroApuesta.text = apuesta.ToString();
        } else{
            apuesta -= 10;
            dineroApuesta.text = apuesta.ToString();
        }
    }
}
