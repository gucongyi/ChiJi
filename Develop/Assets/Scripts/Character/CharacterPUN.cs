namespace CatsAndDogs {
    public class CharacterPUN : Photon.PunBehaviour {

        public override void OnPhotonInstantiate(PhotonMessageInfo info) {
            info.sender.TagObject = gameObject;
        }
    }
}
