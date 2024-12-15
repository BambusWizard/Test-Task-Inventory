using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class BackpackNetwork : MonoBehaviour
{
    private const string URL = "https://wadahub.manerai.com/api/inventory/status";
    private const string TOKEN = "kPERnYcWAY46xaSy8CEzanosAgsWM84Nx7SKM4QBSqPq6c7StWfGxzhxPfDh8MaP";

    public void OnItemAdd(GameObject item) => PrepareRequest(item, true);

    public void OnItemRemove(GameObject item) => PrepareRequest(item, false);

    private void PrepareRequest(GameObject item, bool isAdded)
    {
        var itemData = item.GetComponent<ItemController>().GetItemData();
        
        var itemActionString = isAdded ? "Added" : "Removed";
        var itemDataString = $"{itemActionString} {itemData.id} ({itemData.name})";
        
        StartCoroutine(Request(itemDataString));
    }
    
    // Простой POST запрос на сервер при изменении состояния рюкзака
    private IEnumerator Request(string data)
    {
        var form = new WWWForm();
        form.AddField("ItemData", data);

        using (var request = UnityWebRequest.Post(URL, form))
        {
            request.SetRequestHeader("Authorization", $"Bearer {TOKEN}");
        
            yield return request.SendWebRequest();
        
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
            }
        }
    }
}
