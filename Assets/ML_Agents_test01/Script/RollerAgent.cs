using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class RollerAgent : Agent
{
    public Transform target; // Target��Transform
    Rigidbody rBody; // RollerAgent��RigidBody


    public override void Initialize()
    {
        // RollerAgent��RigidBody�̎Q�Ƃ̎擾
        this.rBody = GetComponent<Rigidbody>();
    }


    // �G�s�\�[�h�J�n���ɌĂ΂��
    public override void OnEpisodeBegin()
    {
        // RollerAgent�������痎�����Ă��鎞
        if (this.transform.localPosition.y < 0)
        {
            // RollerAgent�̈ʒu�Ƒ��x�����Z�b�g
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
        }

        // Target�̈ʒu�̃��Z�b�g
        target.localPosition = new Vector3(
            Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }


    // ��Ԏ擾���ɌĂ΂��
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition.x); //Target��X���W
        sensor.AddObservation(target.localPosition.z); //Target��Z���W
        sensor.AddObservation(this.transform.localPosition.x); //RollerAgent��X���W
        sensor.AddObservation(this.transform.localPosition.z); //RollerAgent��Z���W
        sensor.AddObservation(rBody.velocity.x); // RollerAgent��X���x
        sensor.AddObservation(rBody.velocity.z); // RollerAgent��Z���x
    }


    // �s�����s���ɌĂ΂��
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // RollerAgent�ɗ͂�������
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * 10);

        // RollerAgent��Target�̈ʒu�ɂ��ǂ������
        float distanceToTarget = Vector3.Distance(
            this.transform.localPosition, target.localPosition);
        if (distanceToTarget < 1.42f)
        {
            AddReward(1.0f);
            EndEpisode();
        }

        // RollerAgent�������痎��������
        if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }

    // �q���[���X�e�B�b�N���[�h�̍s�����莞�ɌĂ΂��
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}

