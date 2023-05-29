using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class PlayerDisplayModelState : MyStateMachine
{
    private float m_timer = 2f;
    private Vector3 m_modelPrevPos = Vector3.zero;
    private GameObject m_selectedPlayField = null;


    public Player m_player;
    public PlayerDisplayModelState(Player player)
    {
        m_player = player;
    }

    protected override void DoBehavior()
    {
        Debug.Log("DISPLAY_MODEL");

        if (!m_player.IsMyTurn)
        {
            ExitState = true;
            return;
        }

        GameObject prevHit = m_selectedPlayField;
        m_selectedPlayField = GetRayCastHit();

        if (m_selectedPlayField != null && m_selectedPlayField.tag == "PlayField")
        {
            Vector3 euler = m_selectedPlayField.transform.rotation.eulerAngles;
            euler.x = 0f;
            euler.z = 0f;
            ShowModel(m_selectedPlayField.transform.position, Quaternion.Euler(euler));
        }
        else
        {
            m_selectedPlayField = prevHit;
        }

        if (m_selectedPlayField != null && m_modelPrevPos != Vector3.zero && Vector3.Distance(m_player.CardChose.Model.transform.position, m_modelPrevPos) <= 0.3f)
        {
            if (m_timer <= 0f)
            {
                Debug.Log("Set up model done");
                m_player.CardChose.Model.transform.SetParent(m_selectedPlayField.transform);
                PlayField playField = m_selectedPlayField.GetComponent<PlayField>();
                playField.TurnOnOccupiedField();
                playField.PlayFieldCard = m_player.CardChose;

                ExitState = true;

                return;
            }
            else
            {
                m_timer -= Time.deltaTime;
            }
        }
        else
        {
            m_timer = 2f;
        }
        m_modelPrevPos = m_player.CardChose.Model.transform.position;
    }

    protected override void Exit()
    {
        m_timer = 2f;
        m_modelPrevPos = Vector3.zero;
        m_selectedPlayField = null;

        m_player.CardChose = null;

        if (!m_player.IsMyTurn)
        {
            m_player.EndMyTurn();
            m_player.SwitchState(PlayerStateEnum.WAIT);
        }
        else
        {
            m_player.SwitchState(PlayerStateEnum.CHOOSE_CARD);
        }
    }

    protected override void Initialize()
    {
        Debug.Log("timer: " + m_timer);
        Debug.Log("m_selectedPlayField: " + m_selectedPlayField);
        Debug.Log("m_PrevPos: " + m_modelPrevPos);

        StateInitialized = true;
    }


    private GameObject GetRayCastHit()
    {
        foreach (var source in MixedRealityToolkit.InputSystem.DetectedInputSources)
        {
            // Ignore anything that is not a hand because we want articulated hands
            if (source.SourceType == Microsoft.MixedReality.Toolkit.Input.InputSourceType.Hand)
            {
                foreach (var p in source.Pointers)
                {
                    if (p is IMixedRealityNearPointer)
                    {
                        // Ignore near pointers, we only want the rays
                        continue;
                    }
                    if (p.Result != null)
                    {
                        var startPoint = p.Position;
                        var endPoint = p.Result.Details.Point;
                        var hitObject = p.Result.Details.Object;
                        if (hitObject)
                        {
                            return hitObject;
                        }
                    }

                }
            }
        }
        return null;
    }


    public void ShowModel(Vector3 position, Quaternion rotation)
    {
        Debug.Log("ShowModel");
        if (!m_player.CardChose.Model.activeSelf)
        {
            m_player.CardChose.Model.SetActive(true);
        }
        m_player.CardChose.Model.transform.position = position;
        m_player.CardChose.Model.transform.rotation = rotation;

    }

}
