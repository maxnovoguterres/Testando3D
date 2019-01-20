using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class ECSUtils
    {
        public static void SetComponentData<T>(this Entity entity, T newData, EntityManager entityManager = null) where T : struct, IComponentData
        {
            EntityManager eM = entityManager ?? GameManager.entityManager;

            T data = eM.GetComponentData<T>(entity);
            data = newData;
            eM.SetComponentData<T>(entity, data);
        }

        public static void SetComponentData<T>(this Entity entity, EntityManager entityManager = null, params KeyValuePair<string, object>[] newDatas) where T : struct, IComponentData
        {
            EntityManager eM = entityManager ?? GameManager.entityManager;

            T data = eM.GetComponentData<T>(entity);
            TypedReference trData = __makeref(data);

            for (var i = 0; i < newDatas.Length; i++)
            {
                try
                {
                    data.GetType().GetField(newDatas[i].Key).SetValueDirect(trData, newDatas[i].Value);
                }
                catch
                {
                    Debug.Log($"Erro ao alterar o dado {newDatas[i].Key} da Entidade {entity}");
                }
            }
            eM.SetComponentData(entity, data);
        }
    }
}
