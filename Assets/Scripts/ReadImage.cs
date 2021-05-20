using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct BloqueMapa 
{
    public float rotacion;
    public Vector3 posicion;
    public Vector3 tamaño;

    public BloqueMapa(int rotacion, Vector3 posicion, Vector3 tamaño) : this()
    {
        this.rotacion = rotacion;
        this.posicion = posicion;
        this.tamaño = tamaño;
    }
}

public class ReadImage : MonoBehaviour
{
    [SerializeField]
    private Texture2D imagenMapa;
    private Texture2D image;

    [SerializeField]
    private GameObject wallObject;

    [SerializeField]
    private GameObject groundObject;

    List<BloqueMapa> bloquesASpawnear;

    int[,] matriz;
    private void Start()
    {
        bloquesASpawnear = new List<BloqueMapa>();

        escanearImagenHaciaMatriz();

        buscarYMarcarLineasX();

        buscarYMarcarLineasY();

        instanciarBloques();
    }

    void instanciarBloques() 
    {
        foreach (BloqueMapa unBloque in bloquesASpawnear)
        {
            GameObject bloqueInstanciado = Instantiate(wallObject, unBloque.posicion, Quaternion.identity);

            bloqueInstanciado.transform.localScale = unBloque.tamaño;
        }
    }

    int filas;
    int columnas;

    void printearBloques() 
    {
        if (bloquesASpawnear.Count == 0) 
        {
            Debug.LogError("No hay bloques");
            return;
        }
            
        int contador = 0;

        foreach (BloqueMapa unBloque in bloquesASpawnear) 
        {
            Debug.Log("Bloque numero: " + contador);
            Debug.Log("Rotacion: " + unBloque.rotacion);
            Debug.Log("Posicion: " + unBloque.posicion.ToString());
            Debug.Log("Tamaño: " + unBloque.tamaño.ToString());

            contador++;
        }
    }

    void buscarYMarcarLineasY() 
    {
        int repeticiones = 0;

        while (buscarCandidatoY())
        {
            if (repeticiones > 300000)
            {
                Debug.LogError("[buscarYMarcarLineasX] Demasiadas repeticiones!!!");
                break;
            }

            repeticiones++;
        }
    }

    void buscarYMarcarLineasX()
    {
        // 2 - Leer matriz hasta encontrar primera coincidencia (x)
        // 3 - Buscar final de linea (x)
        // 4 - Guardar linea (x)
        // 5 - Repetir desde paso dos hasta no encontrar mas lineas

        int repeticiones = 0;

        while (buscarCandidatoX()) 
        {
            if (repeticiones > 300000) 
            {
                Debug.LogError("[buscarYMarcarLineasX] Demasiadas repeticiones!!!");
                break;
            }
    
            repeticiones++;
        }
    }

    bool buscarCandidatoY() 
    {
        for (int unaFila = 0; unaFila < filas; unaFila++)
        //for (int unaFila = filas - 1; unaFila >= 0; unaFila--)
        {
            for (int unaColumna = 0; unaColumna < columnas; unaColumna++)
            {
                // Es un candidato a fila (x)
                if (matriz[unaFila, unaColumna] == 2)
                {
                    buscarColumnaEnPos(unaFila, unaColumna);
                    return true;
                }
            }
        }

        return false;
    }

    bool buscarCandidatoX() 
    {
        for (int unaFila = 0; unaFila < filas; unaFila++)
        //for (int unaFila = filas - 1; unaFila >= 0; unaFila--)
        {
            for (int unaColumna = 0; unaColumna < columnas; unaColumna++)
            {
                // Es un candidato a fila (x)
                if (matriz[unaFila, unaColumna] == 1)
                {
                    buscarFilaEnPos(unaFila, unaColumna);
                    return true;
                }
            }
        }

        return false;
    }

    void buscarColumnaEnPos(int fila, int columna) 
    {
        int limiteMin = fila;
        int limiteMax = fila;

        int filaActual = fila;

        int repeticiones = 0;

        while (tieneUnVecinoY(filaActual, columna))
        {
            matriz[filaActual, columna] = 0;
            matriz[filaActual + 1, columna] = 0;
            filaActual++;
            limiteMax = filaActual;

            if (repeticiones > 300000)
            {
                Debug.LogError("[buscarFilaEnPos] Demasiadas repeticiones!!!");
                break;
            }

            repeticiones++;
        }

        float tamañoY = limiteMax - limiteMin + 1;
        float posY = limiteMin + tamañoY / 2;

        BloqueMapa bloqueActual = new BloqueMapa(90, new Vector3(columna + 0.5f, filas - 1 - posY + 0.5f, 0), new Vector3(1, tamañoY, 1));

        bloquesASpawnear.Add(bloqueActual);
    }

    void buscarFilaEnPos(int fila, int columna) 
    {
        int limiteMin = columna;
        int limiteMax = columna;

        int columnaActual = columna;

        int repeticiones = 0;

        while (tieneUnVecinoX(fila, columnaActual)) 
        {
            matriz[fila, columnaActual] = 0;
            matriz[fila, columnaActual + 1] = 0;
            columnaActual ++;
            limiteMax = columnaActual;

            if (repeticiones > 300000)
            {
                Debug.LogError("[buscarFilaEnPos] Demasiadas repeticiones!!!");
                break;
            }

            repeticiones++;
        }

        if (limiteMax == limiteMin)
        {
            matriz[fila, columnaActual] = 2;
            return;
        }
            
        float tamañoX = limiteMax - limiteMin + 1;
        float posX = limiteMin + tamañoX / 2;

        BloqueMapa bloqueActual = new BloqueMapa(0, new Vector3(posX, filas - 1 - fila,0), new Vector3(tamañoX,1,1));

        bloquesASpawnear.Add(bloqueActual);
    }

    bool tieneUnVecinoY(int fila, int columna) 
    {
        return (fila + 1 < filas && matriz[fila + 1, columna] == 2);
    }

    bool tieneUnVecinoX(int fila, int columna)
    {
        return (columna + 1 < columnas && matriz[fila, columna + 1] == 1);
    }

    void escanearImagenHaciaMatriz() 
    {
        image = imagenMapa;
        Color[] pix = image.GetPixels();

        filas = image.height;
        columnas = image.width;

        matriz = new int[filas, columnas];

        int contador = 0;

        for (int unaFila = filas - 1; unaFila >= 0; unaFila--) 
        {
            for (int unaColumna = 0; unaColumna < columnas; unaColumna++)
            {
                Color colorActual = pix[contador];

                int valor = 0;

                if (colorActual.Equals(Color.white))
                    valor = 1;

                matriz[unaFila, unaColumna] = valor;

                contador++;
            }
        }
    }

    void printearMatriz() 
    {
        string stringMatriz = "";

        for (int unafila = 0; unafila < filas; unafila++)
        {
            for (int unacolumna = 0; unacolumna < columnas; unacolumna++) 
            {
                stringMatriz += string.Format("{0} ", matriz[unafila, unacolumna]);
            }
   
            stringMatriz += System.Environment.NewLine + System.Environment.NewLine;
        }

        Debug.Log(stringMatriz);
    }

    // 1 - Escanear imagen hacia matriz
    // 2 - Leer matriz hasta encontrar primera coincidencia (x)
    // 3 - Buscar final de linea (x)
    // 4 - Guardar linea (x)
    // 5 - Repetir desde paso dos hasta no encontrar mas lineas

    // 6 - Leer matriz hasta encontrar primera coincidencia (y)
    // 7 - Buscar final de linea (7)
    // 8 - Guardar linea (y)
    // 9 - Repetir desde paso dos hasta no encontrar mas lineas

    // 10 - Instanciar todas las lineas guardadas como objetos
}
